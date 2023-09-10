from typing import List

from .position import Position
from .block import Block


class Section:
    def __init__(self, position: Position[int], block_id_list: List[int]):
        if position.x % 16 != 0 or position.y % 16 != 0 or position.z % 16 != 0:
            raise ValueError("The position must be a multiple of 16.")

        if len(block_id_list) != 4096:
            raise ValueError("The block ID list must have a length of 4096.")

        self._block_id_list = block_id_list
        self._position = position

    def __getitem__(self, position: Position[int]) -> Block:
        if position.x < 0 or position.x > 15 \
           or position.y < 0 or position.y > 15 \
           or position.z < 0 or position.z > 15:
            raise IndexError("The position is out of range.")

        return Block(self._block_id_list[position.x * 256 + position.y * 16 + position.z],
                      position)

    def __setitem__(self, position: Position[int], block: Block):
        if position.x < 0 or position.x > 15 \
           or position.y < 0 or position.y > 15 \
           or position.z < 0 or position.z > 15:
            raise IndexError("The position is out of range.")

        self._block_id_list[position.x * 256 + position.y * 16 + position.z] = block.get_type_id()

    def get_position(self) -> Position[int]:
        return self._position
