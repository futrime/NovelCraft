"""The block source APIs.

The block source API is the API that can be used to get information about
blocks.
"""
from __future__ import annotations

import math
from abc import ABC, abstractmethod
from typing import Dict

from .block import IBlock, Block
from .position import Position
from .section import Section


class IBlockSource(ABC):
    """The block source interface.

    The block source interface is the interface that can be used to get
    information about blocks.
    """
    @abstractmethod
    def __getitem__(self, position: Position[int]) -> IBlock:
        """Gets a block.

        Args:
            position (Position[int]): The position of the block.

        Returns:
            The block.
        """
        raise NotImplementedError()


class BlockSource(IBlockSource):
    _DEFAULT_BLOCK_ID = -1

    def __init__(self):
        super().__init__()

        self._section_dictionary: Dict[Position[int], Section] = {}

    def __getitem__(self, position: Position[int]) -> Block | None:
        section = self._get_section(position)
        if section is None:
            return None

        return section[Position[int](position.x - section.get_position().x,
                                position.y - section.get_position().y,
                                position.z - section.get_position().z)]

    def __setitem__(self, position: Position[int], block: Block) -> None:
        section = self._get_section(position)
        if section is None:
            section = Section(
                position, [BlockSource._DEFAULT_BLOCK_ID] * 4096)
            self.add_section(section)

        section[Position[int](position.x - section.get_position().x,
                         position.y - section.get_position().y,
                         position.z - section.get_position().z)] = block

    def add_section(self, section: Section) -> None:
        self._section_dictionary[section.get_position()] = section

    def clear(self) -> None:
        self._section_dictionary.clear()

    def remove_section(self, position: Position[int]) -> None:
        position = Position[int](16 * int(math.floor(position.x / 16)),
                            16 * int(math.floor(position.y / 16)),
                            16 * int(math.floor(position.z / 16)))

        self._section_dictionary.pop(position, None)

    def _get_section(self, position: Position[int]) -> Section | None:
        position = Position[int](16 * int(math.floor(position.x / 16)),
                            16 * int(math.floor(position.y / 16)),
                            16 * int(math.floor(position.z / 16)))

        return self._section_dictionary.get(position, None)
