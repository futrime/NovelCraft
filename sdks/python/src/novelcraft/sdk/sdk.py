"""The SDK APIs.

Here is an example of how to use the SDK APIs:
```python
import novelcraft.sdk as sdk

agent = sdk.get_agent()
```
"""

from __future__ import annotations

import argparse
import asyncio
from dataclasses import dataclass
from datetime import datetime

from .agent import IAgent, Agent
from .block import Block
from .block_source import IBlockSource, BlockSource
from .client import Client
from .entity import Entity
from .entity_source import IEntitySource, EntitySource
from .item_stack import ItemStack
from .logger import ILogger, Logger
from .message import Message
from .orientation import Orientation
from .position import Position
from .section import Section
from .inventory import Inventory


@dataclass
class _TickInfo:
    """The tick info."""
    tick: int
    time: datetime


GET_INFO_INTERVAL = 10
PING_INVERVAL = 10

_agent: Agent | None = None
_block_source: BlockSource | None = None
_client: Client | None = None
_entity_source: EntitySource | None = None
_tick_info: _TickInfo | None = None
_task_list: list[asyncio.Task] = []
_token: str | None = None
_tps: float | None = None
_sdk_logger: Logger = Logger('SDK')
_user_logger: Logger = Logger('User')


async def finalize() -> None:
    """Finalizes the SDK.

    This function should be called when the program is about to exit.
    """
    _sdk_logger.info('Finalizing SDK...')

    if _client is not None:
        await _client.stop()

    for task in _task_list:
        task.cancel()


async def initialize() -> None:
    """Initializes the SDK.

    This function should be called before any other SDK APIs are used.
    """
    try:
        global _token, _client

        _sdk_logger.info('Initializing SDK...')

        parser = argparse.ArgumentParser()
        parser.add_argument('--host', type=str, default='localhost')
        parser.add_argument('--port', type=int, default=14514)
        parser.add_argument('--token', type=str, default='')
        args = parser.parse_args()

        _client = Client(args.host, args.port)
        _client.register_message_handler(_message_handler)
        await _client.run()

        _token = args.token

        async def get_info_loop():
            while True:
                try:
                    await _get_info()
                except Exception as e:
                    _sdk_logger.error(f'Failed to get info: {e}')
                await asyncio.sleep(GET_INFO_INTERVAL)

        _task_list.append(asyncio.create_task(get_info_loop()))

    except Exception as e:
        _sdk_logger.error(f'Failed to initialize SDK: {e}')
        raise e


def get_agent() -> IAgent | None:
    """Gets the agent.

    Returns:
        The agent. None if the agent is not initialized.
    """
    return _agent


def get_blocks() -> IBlockSource | None:
    """Gets the block source.

    Returns:
        The block source. None if the block source is not initialized.
    """
    return _block_source


def get_entities() -> IEntitySource | None:
    """Gets the entity source.

    Returns:
        The entity source. None if the entity source is not initialized.
    """
    return _entity_source


def get_logger() -> ILogger:
    """Gets the user logger.

    Returns:
        The user logger.
    """
    return _user_logger


def get_tick() -> int | None:
    """Gets the current tick.

    Returns:
        The tick. None if the tick is not initialized.
    """
    if _tick_info is None:
        return None

    if _tps is None:
        return _tick_info.tick

    seconds_since_last_tick = (
        datetime.now() - _tick_info.time).total_seconds()
    estimated_ticks_since_last_tick = int(seconds_since_last_tick * _tps)

    return _tick_info.tick + estimated_ticks_since_last_tick


def get_tps() -> float | None:
    """Gets the current TPS.

    Returns:
        The TPS. None if the TPS is not initialized.
    """
    return _tps


def _message_handler(message: Message) -> None:
    try:
        global _block_source, _entity_source, _agent, _tick_info, _tps

        if message.get_bound_to() != Message.BoundToKind.CLIENT_BOUND.value:
            return

        if message.get_type() == Message.MessageKind.ERROR.value:
            _sdk_logger.error(
                f'The server returned an error: {message.get_obj()["message"]}')

        elif message.get_type() == Message.MessageKind.AFTER_BLOCK_CHANGE.value:
            if _block_source is None:
                return

            for block_change in message.get_obj()['change_list']:
                position = Position[int](block_change['position']['x'],
                                         block_change['position']['y'],
                                         block_change['position']['z'])
                _block_source[position] = Block(type_id=block_change['block_type_id'],
                                                 position=position)

        elif message.get_type() == Message.MessageKind.AFTER_ENTITY_CREATE.value:
            if _entity_source is None:
                return

            for creation in message.get_obj()['creation_list']:
                _entity_source.add_entity(Entity(type_id=creation['entity_type_id'],
                                                  unique_id=creation['unique_id'],
                                                  position=Position[float](creation['position']['x'],
                                                                           creation['position']['y'],
                                                                           creation['position']['z']),
                                                  orientation=Orientation[float](creation['orientation']['yaw'],
                                                                                 creation['orientation']['pitch'])))

        elif message.get_type() == Message.MessageKind.AFTER_ENTITY_ORIENTATION_CHANGE.value:
            if _entity_source is None:
                return

            for orientation_change in message.get_obj()['change_list']:
                entity = _entity_source[orientation_change['unique_id']]
                if entity is None:
                    continue

                entity.set_orientation(Orientation[float](orientation_change['orientation']['yaw'],
                                                          orientation_change['orientation']['pitch']))

                if _agent is not None and _agent.get_unique_id() == entity.get_unique_id():
                    _agent.set_orientation(entity.get_orientation())

        elif message.get_type() == Message.MessageKind.AFTER_ENTITY_POSITION_CHANGE.value:
            if _entity_source is None:
                return

            for position_change in message.get_obj()['change_list']:
                entity = _entity_source[position_change['unique_id']]
                if entity is None:
                    continue

                entity.set_position(Position[float](position_change['position']['x'],
                                                    position_change['position']['y'],
                                                    position_change['position']['z']))

                if _agent is not None and _agent.get_unique_id() == entity.get_unique_id():
                    _agent.set_position(entity.get_position())

        elif message.get_type() == Message.MessageKind.AFTER_ENTITY_REMOVE.value:
            if _entity_source is None:
                return

            for removal in message.get_obj()['removal_id_list']:
                _entity_source.remove_entity(removal)

        elif message.get_type() == Message.MessageKind.AFTER_PLAYER_INVENTORY_CHANGE.value:
            if _agent is None:
                return

            for change in message.get_obj()['change_list']:
                if change['player_unique_id'] is not _agent.get_unique_id():
                    continue

                for slot_change in change['change_list']:
                    _agent.get_inventory()[slot_change['slot']] = None if slot_change['item_type_id'] is None \
                        else ItemStack(type_id=slot_change['item_type_id'],
                                        count=slot_change['count'])

        elif message.get_type() == Message.MessageKind.GET_BLOCKS_AND_ENTITIES.value:
            _block_source = BlockSource()
            _entity_source = EntitySource()

            for section in message.get_obj()['sections']:
                _block_source.add_section(Section(position=Position[int](section['position']['x'],
                                                                          section['position']['y'],
                                                                          section['position']['z']),
                                                   block_id_list=section['blocks']))

            for entity in message.get_obj()['entities']:
                _entity_source.add_entity(Entity(type_id=entity['type_id'],
                                                  unique_id=entity['unique_id'],
                                                  position=Position[float](entity['position']['x'],
                                                                           entity['position']['y'],
                                                                           entity['position']['z']),
                                                  orientation=Orientation[float](entity['orientation']['yaw'],
                                                                                 entity['orientation']['pitch'])))

                if _agent is not None and _agent.get_unique_id() == entity['unique_id']:
                    _agent.set_position(Position[float](entity['position']['x'],
                                                        entity['position']['y'],
                                                        entity['position']['z']))
                    _agent.set_orientation(Orientation[float](entity['orientation']['yaw'],
                                                              entity['orientation']['pitch']))

        elif message.get_type() == Message.MessageKind.GET_PLAYER_INFO.value:
            if _client is None:
                return
            if _token is None:
                return

            player_info = message.get_obj()
            if _agent is None:
                # Create a new agent
                _agent = Agent(client=_client,
                                token=_token,
                                unique_id=player_info["unique_id"],
                                position=Position[float](
                                    player_info["position"]['x'],
                                    player_info["position"]['y'],
                                    player_info["position"]['z']
                                ),
                                orientation=Orientation[float](
                                    player_info['orientation']['yaw'],
                                    player_info['orientation']['pitch']
                                )
                                )

            # Update Info
            _agent.set_health(player_info['health'])
            _agent.set_position(Position[float](
                player_info["position"]['x'],
                player_info["position"]['y'],
                player_info["position"]['z']
            ))
            _agent.set_orientation(Orientation[float](player_info['orientation']['yaw'],
                                                      player_info['orientation']['pitch']))
            # Main slots
            _agent.get_inventory()._main_hand_slot = int(
                player_info['main_hand'])
            # inventory
            for slot in range(Inventory._SLOTS):
                item_stack = player_info['inventory'][slot]
                if item_stack is not None:
                    _agent.get_inventory()[slot] = ItemStack(
                        item_stack['type_id'], item_stack['count'])

        elif message.get_type() == Message.MessageKind.GET_TICK.value:
            tick_message = message.get_obj()
            _tick_info = _TickInfo(tick_message['tick'], datetime.now())
            _tps = tick_message['ticks_per_second']

    except Exception as e:
        _sdk_logger.error(f'Error while handling message: {e}')


async def _get_info():
    global _client
    if _client is None:
        return

    # Send get tick message
    _client.send(Message({
        "bound_to": Message.BoundToKind.SERVER_BOUND.value,
        "type": Message.MessageKind.GET_TICK.value,
        "token": _token,
    }))

    # Send get player info message
    _client.send(Message({
        "bound_to": Message.BoundToKind.SERVER_BOUND.value,
        "type": Message.MessageKind.GET_PLAYER_INFO.value,
        "token": _token,
    }))

    # Compute near section
    if _agent is not None:
        agent_position = _agent.get_position()
        agent_position = _agent.get_position()

        request_section_list = [{
            'x': int(agent_position.x + x*16),
            'y': int(agent_position.y + y*16),
            'z': int(agent_position.z + z*16)
        } for x in range(-1, 2) for y in range(-1, 2) for z in range(-1, 2)]

        # Send get blocks and entities message
        _client.send(Message({
            "bound_to": Message.BoundToKind.SERVER_BOUND.value,
            "type": Message.MessageKind.GET_BLOCKS_AND_ENTITIES.value,
            "token": _token,
            "request_section_list": request_section_list})
        )
