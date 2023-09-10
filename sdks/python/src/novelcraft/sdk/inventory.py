"""The inventory APIs.

The inventory API is the API that can be used to get information about
inventories."""
from __future__ import annotations

from abc import ABC, abstractmethod
from typing import List

from .item_stack import IItemStack, ItemStack
from .message import Message
from .client import Client


class IInventory(ABC):
    """The inventory interface.

    The inventory interface is the interface that can be used to get
    information about inventories and set items in them.
    """
    @abstractmethod
    def __getitem__(self, slot: int) -> IItemStack:
        """Gets an item from the inventory.

        Args:
            slot (int): The slot of the item.

        Returns:
            The item.
        """
        raise NotImplementedError()

    @abstractmethod
    def get_main_hand_slot(self) -> int:
        """Gets the main hand slot.

        Returns:
            The main hand slot.
        """
        raise NotImplementedError()

    @abstractmethod
    def set_main_hand_slot(self, slot: int) -> None:
        """Sets the main hand slot.

        Args:
            slot (int): The main hand slot.
        """
        raise NotImplementedError()

    @abstractmethod
    def craft(self, ingredients: List[int | None]) -> None:
        """Crafts an item.

        Args:
            ingredients (List[int]): The ingredients.
        """
        raise NotImplementedError()

    @abstractmethod
    def drop_item(self, slot: int, count: int) -> None:
        """Drops an item.

        Args:
            slot (int): The slot of the item.
            count (int): The count of the item.
        """
        raise NotImplementedError()

    @abstractmethod
    def merge_slots(self, from_slot: int, to_slot: int) -> None:
        """Merges two slots.

        Args:
            from_slot (int): The slot to merge from.
            to_slot (int): The slot to merge to.
        """
        raise NotImplementedError()

    @abstractmethod
    def swap_slots(self, slot1: int, slot2: int) -> None:
        """Swaps two slots.

        Args:
            slot1 (int): The first slot.
            slot2 (int): The second slot.
        """
        raise NotImplementedError()


class Inventory(IInventory):
    _HOTBAR_SLOTS = 9
    _SLOTS = 36

    def __init__(self, client: Client, token: str):
        super().__init__()

        self._client = client
        self._item_stack_list: List[ItemStack | None] = [
            None] * Inventory._SLOTS
        self._main_hand_slot: int = 0
        self._token: str = token

    def __getitem__(self, slot: int) -> ItemStack | None:
        if slot < 0 or slot >= Inventory._SLOTS:
            raise IndexError("Slot is out of range")

        return self._item_stack_list[slot]

    def __setitem__(self, slot: int, item_stack: ItemStack | None) -> None:
        if slot < 0 or slot >= Inventory._SLOTS:
            raise IndexError("Slot is out of range")

        # Copy the item stack to prevent the item stack from being modified
        self._item_stack_list[slot] = ItemStack(item_stack.get_type_id(), item_stack.get_count()) \
            if item_stack is not None else None

    def get_main_hand_slot(self) -> int:
        return self._main_hand_slot

    def set_main_hand_slot(self, slot: int) -> None:
        client = self._client
        if client is None:
            raise RuntimeError("Client is not running")

        if slot < 0 or slot >= Inventory._HOTBAR_SLOTS:
            raise IndexError("Slot is out of range")

        self._main_hand_slot = slot

        client.send(Message({
            "bound_to": Message.BoundToKind.SERVER_BOUND.value,
            "type": Message.MessageKind.PERFORM_SWITCH_MAIN_HAND_SLOT.value,
            "token": self._token,
            "new_main_hand": slot
        }))

    def craft(self, ingredients: List[int | None]) -> None:
        client = self._client
        if client is None:
            raise RuntimeError("Client is not running")

        client.send(Message({
            "bound_to": Message.BoundToKind.SERVER_BOUND.value,
            "type": Message.MessageKind.PERFORM_CRAFT.value,
            "token": self._token,
            "item_id_sequence": ingredients
        }))

    def drop_item(self, slot: int, count: int) -> None:
        client = self._client
        if client is None:
            raise RuntimeError("Client is not running")

        if slot < 0 or slot >= Inventory._SLOTS:
            raise IndexError("Slot is out of range")

        client.send(Message({
            "bound_to": Message.BoundToKind.SERVER_BOUND.value,
            "type": Message.MessageKind.PERFORM_DROP_ITEM.value,
            "token": self._token,
            "drop_items": [{
                "slot": slot,
                "count": count
            }]
        }))

    def merge_slots(self, from_slot: int, to_slot: int) -> None:
        client = self._client
        if client is None:
            raise RuntimeError("Client is not running")

        if from_slot < 0 or from_slot >= Inventory._SLOTS:
            raise IndexError("From slot is out of range")
        if to_slot < 0 or to_slot >= Inventory._SLOTS:
            raise IndexError("To slot is out of range")

        client.send(Message({
            "bound_to": Message.BoundToKind.SERVER_BOUND.value,
            "type": Message.MessageKind.PERFORM_MERGE_SLOTS.value,
            "token": self._token,
            "from_slot": from_slot,
            "to_slot": to_slot
        }))

    def swap_slots(self, slot1: int, slot2: int) -> None:
        client = self._client
        if client is None:
            raise RuntimeError("Client is not running")

        if slot1 < 0 or slot1 >= Inventory._SLOTS:
            raise IndexError("Slot 1 is out of range")
        if slot2 < 0 or slot2 >= Inventory._SLOTS:
            raise IndexError("Slot 2 is out of range")

        client.send(Message({
            "bound_to": Message.BoundToKind.SERVER_BOUND.value,
            "type": Message.MessageKind.PERFORM_SWAP_SLOTS.value,
            "token": self._token,
            "slot_a": slot1,
            "slot_b": slot2
        }))
