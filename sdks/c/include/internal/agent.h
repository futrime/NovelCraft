#pragma once

#ifdef __cplusplus
extern "C" {
#endif

#include "internal/inventory.h"
#include "orientation.h"
#include "position.h"

enum ncsdk_Agent_InteractionKind {
  ncsdk_Agent_InteractionKind_Click,
  ncsdk_Agent_InteractionKind_HoldStart,
  ncsdk_Agent_InteractionKind_HoldEnd
};

enum ncsdk_Agent_MovementKind {
  ncsdk_Agent_MovementKind_Forward,
  ncsdk_Agent_MovementKind_Backward,
  ncsdk_Agent_MovementKind_Left,
  ncsdk_Agent_MovementKind_Right,
  ncsdk_Agent_MovementKind_Stopped
};

struct ncsdk_Agent {
  float health;
  ncsdk_Inventory* inventory;
  enum ncsdk_Agent_MovementKind movement;
  ncsdk_Orientation orientation;
  ncsdk_Position(float) position;
  char* token;
  int type_id;
  int unique_id;
};
typedef struct ncsdk_Agent ncsdk_Agent;

ncsdk_Agent* ncsdk_Agent_New(const char* token, int unique_id,
                             const ncsdk_Position(float) * position,
                             const ncsdk_Orientation* orientation);

void ncsdk_Agent_Delete(ncsdk_Agent* self);

void ncsdk_Agent_Attack(ncsdk_Agent* self,
                        enum ncsdk_Agent_InteractionKind kind);

float ncsdk_Agent_GetHealth(const ncsdk_Agent* self);

const ncsdk_Inventory* ncsdk_Agent_GetInventory(const ncsdk_Agent* self);

enum ncsdk_Agent_MovementKind ncsdk_Agent_GetMovement(const ncsdk_Agent* self);

const ncsdk_Orientation* ncsdk_Agent_GetOrientation(const ncsdk_Agent* self);

const ncsdk_Position(float) * ncsdk_Agent_GetPosition(const ncsdk_Agent* self);

const char* ncsdk_Agent_GetToken(const ncsdk_Agent* self);

int ncsdk_Agent_GetTypeId(const ncsdk_Agent* self);

int ncsdk_Agent_GetUniqueId(const ncsdk_Agent* self);

void ncsdk_Agent_Jump(ncsdk_Agent* self);

void ncsdk_Agent_LookAt(ncsdk_Agent* self,
                        const ncsdk_Position(float) * position);

void ncsdk_Agent_SetMovement(ncsdk_Agent* self,
                             enum ncsdk_Agent_MovementKind movement);

void ncsdk_Agent_Use(ncsdk_Agent* self, enum ncsdk_Agent_InteractionKind kind);

#ifdef __cplusplus
}
#endif
