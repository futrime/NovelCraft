"""The block APIs.

The block API is the API that can be used to get information about blocks.
"""

from abc import ABC, abstractmethod

from .position import Position


class IBlock(ABC):
    """The block interface.

    The block interface is the interface that can be used to get
    information about blocks.
    """
    @abstractmethod
    def get_position(self) -> Position[int]:
        """Gets the position of the block.

        Returns:
            The position.
        """
        raise NotImplementedError()

    @abstractmethod
    def get_type_id(self) -> int:
        """Gets the type ID of the block.

        Returns:
            The type ID.
        """
        raise NotImplementedError()


class Block(IBlock):
    def __init__(self, type_id: int, position: Position[int]):
        super().__init__()

        self._position = position
        self._type_id = type_id

    def get_position(self) -> Position[int]:
        return self._position

    def get_type_id(self) -> int:
        return self._type_id
