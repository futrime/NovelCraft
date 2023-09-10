"""The entity source APIs.

The entity source API is the API that can be used to get
information about entities.
"""

from __future__ import annotations

from abc import ABC, abstractmethod
from typing import Dict, List

from .entity import IEntity, Entity


class IEntitySource(ABC):
    """The entity source interface.
    
    The entity source API is the API that can be used to get information about
    entities.
    """
    @abstractmethod
    def __getitem__(self, unique_id: int) -> IEntity:
        """Gets an entity.
        
        Args:
            unique_id (int): The unique ID of the entity.
        
        Returns:
            The entity.
        """
        raise NotImplementedError()

    @abstractmethod
    def get_all_entities(self) -> List[IEntity]:
        """Gets all entities.
        
        Returns:
            The entities.
        """
        raise NotImplementedError()

class EntitySource(IEntitySource):
    def __init__(self):
        super().__init__()

        self._entity_dictionary: Dict[int, Entity] = {}

    def __getitem__(self, unique_id: int) -> Entity | None:
        return self._entity_dictionary.get(unique_id, None)
    
    def __iter__(self):
        return iter(self.get_all_entities())
    
    def add_entity(self, entity: Entity) -> None:
        self._entity_dictionary[entity.get_unique_id()] = entity

    def clear(self) -> None:
        self._entity_dictionary.clear()

    def get_all_entities(self) -> List[Entity]:
        return list(self._entity_dictionary.values())

    def remove_entity(self, unique_id: int) -> None:
        self._entity_dictionary.pop(unique_id, None)
