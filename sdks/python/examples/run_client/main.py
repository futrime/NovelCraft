import asyncio
import pathlib
import sys

sys.path.append(
    str(pathlib.Path(__file__).parent.parent.parent.resolve()))

from src.novelcraft.sdk import *


async def main():
    await initialize()
    get_logger().info('Hello, world!')
    await asyncio.sleep(30)
    await finalize()
    get_logger().info('Goodbye, world!')

if __name__ == '__main__':
    asyncio.run(main())
