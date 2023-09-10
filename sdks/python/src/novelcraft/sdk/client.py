import asyncio
import json
import queue
from typing import Callable, List

import websockets

from .logger import Logger
from .message import Message


class Client:
    _MESSAGE_TRANSMISSION_INTERVAL = 0.01
    _MESSAGE_QUEUE_CAPACITY = 100

    def __init__(self, host: str, port: int):
        self._connection = None
        self._logger = Logger("SDK.Client")
        self._message_handler_list: List[Callable[[Message], None]] = []
        self._message_queue = queue.Queue(
            maxsize=Client._MESSAGE_QUEUE_CAPACITY)
        self._task_list: List[asyncio.Task] = []
        self._url = f"ws://{host}:{port}"

    def register_message_handler(self, handler: Callable[[Message], None]) -> None:
        self._message_handler_list.append(handler)

    async def run(self) -> None:
        self._connection = await Client._try_connect(self._url)
        self._logger.info(f"Connected to {self._url}")

        self._task_list.append(asyncio.create_task(self._send_loop()))
        self._task_list.append(asyncio.create_task(self._receive_loop()))

    def send(self, message: Message) -> None:
        if self._message_queue.full():
            self._logger.error("Message queue is full, dropping message")
            return

        self._message_queue.put(message)

    async def stop(self) -> None:
        for task in self._task_list:
            task.cancel()

        await self._connection.close() # type: ignore
    
    async def _receive_loop(self) -> None:
        while True:
            try:
                json_string = await self._connection.recv()  # type: ignore
                message = Message(json.loads(json_string))

                handler_list = self._message_handler_list.copy()

                for handler in handler_list:
                    handler(message)

            except Exception as e:
                self._logger.error(f"Failed to receive message: {e}")
                self._logger.info("Trying to reconnect...")
                self._connection = await Client._try_connect(self._url)

    async def _send_loop(self) -> None:
        while True:
            try:
                if not self._message_queue.empty():
                    message: Message = self._message_queue.get()
                    json_string = json.dumps(message.get_obj())

                    try:
                        await self._connection.send(json_string) # type: ignore

                    except Exception as e:
                        self._logger.error(f"Failed to send message: {e}")

                await asyncio.sleep(Client._MESSAGE_TRANSMISSION_INTERVAL)

            except Exception as e:
                self._logger.error(f"Unexpected error occured in send loop: {e}")

    @staticmethod
    async def _try_connect(url: str) -> websockets.WebSocketClientProtocol:  # type: ignore
        logger = Logger("SDK.Client")
        logger.info(f"Trying to connect to {url}")

        is_connected = False
        while not is_connected:
            try:
                return await websockets.connect(url)  # type: ignore
            except:
                logger.error(f"Failed to connect to {url}")
                logger.info("Retrying...")
