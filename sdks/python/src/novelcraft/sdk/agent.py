"""The agent APIs.

The agent API is the API that can be used to control the agent and get
information about the agent.
"""

from abc import ABC, abstractmethod
from enum import Enum

from .entity import IEntity
from .inventory import IInventory, Inventory
from .message import Message
from .orientation import Orientation
from .position import Position
from .client import Client


class IAgent(IEntity, ABC):
    """The agent interface.

    The agent interface is the interface that can be used to control the
    agent and get information about the agent.
    """
    class MovementKind(Enum):
        """The movement kind."""
        STOPPED = 0
        FORWARD = 1
        BACKWARD = 2
        LEFT = 3
        RIGHT = 4

    class InteractionKind(Enum):
        """The interaction kind."""
        CLICK = 0
        HOLD_START = 1
        HOLD_END = 2

    @abstractmethod
    def get_health(self) -> float:
        """Gets the health of the agent.

        Returns:
            The health.
        """
        raise NotImplementedError()

    @abstractmethod
    def get_inventory(self) -> IInventory:
        """Gets the inventory of the agent.

        Returns:
            The inventory.
        """
        raise NotImplementedError()

    @abstractmethod
    def get_movement(self) -> MovementKind:
        """Gets the movement of the agent.

        Returns:
            The movement.
        """
        raise NotImplementedError()

    @abstractmethod
    def set_movement(self, movement: MovementKind) -> None:
        """Sets the movement of the agent.

        Args:
            movement (MovementKind): The movement.
        """
        raise NotImplementedError()

    @abstractmethod
    def attack(self, interaction: InteractionKind) -> None:
        """Attacks.

        Args:
            interaction (InteractionKind): The interaction.
        """
        raise NotImplementedError()

    @abstractmethod
    def jump(self) -> None:
        """Jumps."""
        raise NotImplementedError()

    @abstractmethod
    def look_at(self, position: Position[float]) -> None:
        """Looks at a position.

        Args:
            position (Position[float]): The position.
        """
        raise NotImplementedError()

    @abstractmethod
    def use(self, interaction: InteractionKind) -> None:
        """Uses.

        Args:
            interaction (InteractionKind): The interaction.
        """
        raise NotImplementedError()

class Agent(IAgent):
    def __init__(self, client: Client, token: str, unique_id: int, position: Position[float], orientation: Orientation[float]):
        super().__init__()

        self._client = client
        self._health = 0.0
        self._inventory = Inventory(client=client, token=token)
        self._movement = IAgent.MovementKind.STOPPED
        self._orientation = orientation
        self._position = position
        self._token = token
        self._unique_id = unique_id

    def get_orientation(self) -> Orientation[float]:
        return self._orientation

    def get_position(self) -> Position[float]:
        return self._position

    def get_type_id(self) -> int:
        return 0

    def get_unique_id(self) -> int:
        return self._unique_id

    def set_orientation(self, orientation: Orientation[float]):
        self._orientation = orientation

    def set_position(self, position: Position[float]):
        self._position = position

    def get_health(self) -> float:
        return self._health

    def set_health(self, health: float):
        self._health = health

    def get_inventory(self) -> Inventory:
        return self._inventory

    def get_movement(self) -> IAgent.MovementKind:
        return self._movement

    def set_movement(self, movement: IAgent.MovementKind):
        client = self._client
        if client is None:
            raise RuntimeError("Client is not running")

        self._movement = movement

        client.send(Message({
            "bound_to": Message.BoundToKind.SERVER_BOUND.value,
            "type": Message.MessageKind.PERFORM_MOVE.value,
            "token": self._token,
            "direction": movement.value
        }))

    def attack(self, interaction: IAgent.InteractionKind) -> None:
        client = self._client
        if client is None:
            raise RuntimeError("Client is not running")
        
        client.send(Message({
            "bound_to": Message.BoundToKind.SERVER_BOUND.value,
            "type": Message.MessageKind.PERFORM_ATTACK.value,
            "token": self._token,
            "attack_kind": interaction.value
        }))

    def jump(self) -> None:
        client = self._client
        if client is None:
            raise RuntimeError("Client is not running")
        
        client.send(Message({
            "bound_to": Message.BoundToKind.SERVER_BOUND.value,
            "type": Message.MessageKind.PERFORM_JUMP.value,
            "token": self._token
        }))

    def look_at(self, position: Position[float]) -> None:
        client = self._client
        if client is None:
            raise RuntimeError("Client is not running")
        
        client.send(Message({
            "bound_to": Message.BoundToKind.SERVER_BOUND.value,
            "type": Message.MessageKind.PERFORM_LOOK_AT.value,
            "token": self._token,
            "look_at_position": {
                "x": position.x,
                "y": position.y,
                "z": position.z
            }
        }))

    def use(self, interaction: IAgent.InteractionKind) -> None:
        client = self._client
        if client is None:
            raise RuntimeError("Client is not running")
        
        client.send(Message({
            "bound_to": Message.BoundToKind.SERVER_BOUND.value,
            "type": Message.MessageKind.PERFORM_USE.value,
            "token": self._token,
            "use_kind": interaction.value
        }))
