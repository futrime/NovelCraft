from enum import Enum
from typing import Any, Dict

import jsonschema


class Message:
    class BoundToKind(Enum):
        """The bound to kind."""
        SERVER_BOUND = 0
        CLIENT_BOUND = 1

    class MessageKind(Enum):
        """The message kind."""
        PING = 100

        ERROR = 200

        AFTER_BLOCK_CHANGE = 400
        AFTER_ENTITY_ATTACK = 401
        AFTER_ENTITY_CREATE = 402
        AFTER_ENTITY_DESPAWN = 403
        AFTER_ENTITY_HURT = 404
        AFTER_ENTITY_ORIENTATION_CHANGE = 405
        AFTER_ENTITY_POSITION_CHANGE = 406
        AFTER_ENTITY_REMOVE = 407
        AFTER_ENTITY_SPAWN = 408
        AFTER_PLAYER_INVENTORY_CHANGE = 409

        GET_BLOCKS_AND_ENTITIES = 300
        GET_PLAYER_INFO = 301
        GET_TICK = 302

        PERFORM_ATTACK = 500
        PERFORM_CRAFT = 501
        PERFORM_DROP_ITEM = 502
        PERFORM_JUMP = 503
        PERFORM_MERGE_SLOTS = 504
        PERFORM_MOVE = 505
        PERFORM_LOOK_AT = 506
        PERFORM_ROTATE = 507
        PERFORM_SWAP_SLOTS = 508
        PERFORM_SWITCH_MAIN_HAND_SLOT = 509
        PERFORM_USE = 510

    _JSON_SCHEMA = {
        "$schema": "http://json-schema.org/draft-07/schema#",
        "type": "object",
        "properties": {
            "bound_to": {
                "type": "integer",
                "enum": [
                    BoundToKind.SERVER_BOUND.value,
                    BoundToKind.CLIENT_BOUND.value,
                ]
            },
            "type": {
                "type": "integer",
                "enum": [field.value for field in MessageKind]
            }
        },
        "required": [
            "bound_to",
            "type"
        ]
    }

    def __init__(self, obj: Dict[str, Any]):
        validator = jsonschema.Draft7Validator(Message._JSON_SCHEMA)
        if not validator.is_valid(obj):
            raise ValueError("The JSON is not valid.")

        self._json = obj

    def __getitem__(self, key: str) -> Any:
        return self._json[key]

    def __setitem__(self, key: str, value: Any) -> None:
        self._json[key] = value

    def get_obj(self) -> Dict[str, Any]:
        return self._json

    def get_bound_to(self) -> int:
        return self._json["bound_to"]

    def get_type(self) -> int:
        return self._json["type"]
