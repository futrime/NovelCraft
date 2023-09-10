#pragma once

#ifdef __cplusplus
extern "C" {
#endif

#include <jansson.h>

enum ncsdk_Message_BoundToKind {
  ncsdk_Message_BoundToKind_ServerBound,

  ncsdk_Message_BoundToKind_ClientBound,

  ncsdk_Message_BoundToKind_Unknown
};

enum ncsdk_Message_MessageKind {
  ncsdk_Message_MessageKind_Ping = 100,

  ncsdk_Message_MessageKind_Error = 200,

  ncsdk_Message_MessageKind_AfterBlockChange = 400,
  ncsdk_Message_MessageKind_AfterEntityAttack,
  ncsdk_Message_MessageKind_AfterEntityCreate,
  ncsdk_Message_MessageKind_AfterEntityDespawn,
  ncsdk_Message_MessageKind_AfterEntityHurt,
  ncsdk_Message_MessageKind_AfterEntityOrientationChange,
  ncsdk_Message_MessageKind_AfterEntityPositionChange,
  ncsdk_Message_MessageKind_AfterEntityRemove,
  ncsdk_Message_MessageKind_AfterEntitySpawn,
  ncsdk_Message_MessageKind_AfterPlayerInventoryChange,

  ncsdk_Message_MessageKind_GetBlocksAndEntities = 300,
  ncsdk_Message_MessageKind_GetPlayerInfo,
  ncsdk_Message_MessageKind_GetTick,

  ncsdk_Message_MessageKind_PerformAttack = 500,
  ncsdk_Message_MessageKind_PerformCraft,
  ncsdk_Message_MessageKind_PerformDropItem,
  ncsdk_Message_MessageKind_PerformJump,
  ncsdk_Message_MessageKind_PerformMergeSlots,
  ncsdk_Message_MessageKind_PerformMove,
  ncsdk_Message_MessageKind_PerformLookAt,
  ncsdk_Message_MessageKind_PerformRotate,
  ncsdk_Message_MessageKind_PerformSwapSlots,
  ncsdk_Message_MessageKind_PerformSwitchMainHandSlot,
  ncsdk_Message_MessageKind_PerformUse,

  ncsdk_Message_MessageKind_Unknown
};

struct ncsdk_Message {
  struct json_t* json;
};
typedef struct ncsdk_Message ncsdk_Message;

ncsdk_Message* ncsdk_Message_New(const char* json_string);

ncsdk_Message* ncsdk_Message_NewFromJson(json_t* json);

void ncsdk_Message_Delete(ncsdk_Message* self);

const json_t* ncsdk_Message_GetJson(const ncsdk_Message* self);

/// @note The returned string should be freed by the caller.
char* ncsdk_Message_GetJsonString(const ncsdk_Message* self);

enum ncsdk_Message_BoundToKind ncsdk_Message_GetBoundTo(
    const ncsdk_Message* self);

enum ncsdk_Message_MessageKind ncsdk_Message_GetType(
    const ncsdk_Message* self);

#ifdef __cplusplus
}
#endif
