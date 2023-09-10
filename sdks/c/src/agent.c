#include "internal/agent.h"

#include <jansson.h>
#include <stdlib.h>

#include "internal/client.h"
#include "internal/inventory.h"
#include "internal/message.h"
#include "internal/sdk.h"
#include "orientation.h"
#include "position.h"
#include "string.h"

ncsdk_Agent* ncsdk_Agent_New(const char* token, int unique_id,
                             const ncsdk_Position(float) * position,
                             const ncsdk_Orientation* orientation) {
  ncsdk_Agent* self = malloc(sizeof(ncsdk_Agent));
  self->health = 0.0;
  self->inventory = ncsdk_Inventory_New();
  self->movement = ncsdk_Agent_MovementKind_Stopped;
  self->orientation = *orientation;
  self->position = *position;
  self->token = malloc(strlen(token) + 1);
  strcpy(self->token, token);
  self->type_id = 0;
  self->unique_id = unique_id;
  return self;
}

void ncsdk_Agent_Delete(ncsdk_Agent* self) {
  ncsdk_Inventory_Delete(self->inventory);
  free(self->token);
  free(self);
}

void ncsdk_Agent_Attack(ncsdk_Agent* self,
                        enum ncsdk_Agent_InteractionKind kind) {
  // TODO: Send attack event to server.
  // Construct the attack message
  json_t* attack_json = json_object();
  json_object_set(attack_json, "bound_to",
                  ncsdk_Message_BoundToKind_ServerBound);
  json_object_set(attack_json, "type", ncsdk_Message_MessageKind_PerformAttack);
  json_object_set(attack_json, "token", ncsdk_Agent_GetToken(self));
  json_object_set(attack_json, "attack_kind", json_integer(kind));
  // Send it to server
  ncsdk_Client_Send(ncsdk_GetClient(), ncsdk_Message_NewFromJson(attack_json));
}

float ncsdk_Agent_GetHealth(const struct ncsdk_Agent* self) {
  return self->health;
}

const struct ncsdk_Inventory* ncsdk_Agent_GetInventory(
    const ncsdk_Agent* self) {
  return self->inventory;
}

enum ncsdk_Agent_MovementKind ncsdk_Agent_GetMovement(const ncsdk_Agent* self) {
  return self->movement;
}

const ncsdk_Orientation* ncsdk_Agent_GetOrientation(const ncsdk_Agent* self) {
  return &self->orientation;
}

const ncsdk_Position(float) * ncsdk_Agent_GetPosition(const ncsdk_Agent* self) {
  return &self->position;
}

const char* ncsdk_Agent_GetToken(const ncsdk_Agent* self) {
  return self->token;
}

int ncsdk_Agent_GetTypeId(const ncsdk_Agent* self) { return self->type_id; }

int ncsdk_Agent_GetUniqueId(const ncsdk_Agent* self) { return self->unique_id; }

void ncsdk_Agent_Jump(ncsdk_Agent* self) {
  // TODO: Send jump event to server.
  // Construct the attack message
  json_t* jump_json = json_object();
  json_object_set(jump_json, "bound_to", ncsdk_Message_BoundToKind_ServerBound);
  json_object_set(jump_json, "type", ncsdk_Message_MessageKind_PerformJump);
  json_object_set(jump_json, "token", ncsdk_Agent_GetToken(self));
  // Send it to server
  ncsdk_Client_Send(ncsdk_GetClient(), ncsdk_Message_NewFromJson(jump_json));
}

void ncsdk_Agent_LookAt(ncsdk_Agent* self,
                        const ncsdk_Position(float) * position) {
  // TODO: Send look at event to server.
  // Construct the attack message //
  json_t* look_at_json = json_object();
  json_object_set(look_at_json, "bound_to",
                  ncsdk_Message_BoundToKind_ServerBound);
  json_object_set(look_at_json, "type",
                  ncsdk_Message_MessageKind_PerformLookAt);
  json_object_set(look_at_json, "token", ncsdk_Agent_GetToken(self));
  // Position
  json_t* array = json_array();
  json_array_append_new(array, json_real((double)position->x));
  json_array_append_new(array, json_real((double)position->y));
  json_array_append_new(array, json_real((double)position->z));
  json_object_set_new(look_at_json, "look_at_position", array);
  // Send it to server //
  ncsdk_Client_Send(ncsdk_GetClient(), ncsdk_Message_NewFromJson(look_at_json));
}

void ncsdk_Agent_SetMovement(ncsdk_Agent* self,
                             enum ncsdk_Agent_MovementKind movement) {
  // TODO: Send movement event to server.
  // Construct the attack message
  json_t* move_json = json_object();
  json_object_set(move_json, "bound_to", ncsdk_Message_BoundToKind_ServerBound);
  json_object_set(move_json, "type", ncsdk_Message_MessageKind_PerformMove);
  json_object_set(move_json, "token", ncsdk_Agent_GetToken(self));
  json_object_set(move_json, "direction", json_integer(movement));
  // Send it to server
  ncsdk_Client_Send(ncsdk_GetClient(), ncsdk_Message_NewFromJson(move_json));
}

void ncsdk_Agent_Use(ncsdk_Agent* self, enum ncsdk_Agent_InteractionKind kind) {
  // TODO: Send use event to server.
  // Construct the attack message
  json_t* use_json = json_object();
  json_object_set(use_json, "bound_to", ncsdk_Message_BoundToKind_ServerBound);
  json_object_set(use_json, "type", ncsdk_Message_MessageKind_PerformUse);
  json_object_set(use_json, "token", ncsdk_Agent_GetToken(self));
  json_object_set(use_json, "use_kind", json_integer(kind));
  // Send it to server
  ncsdk_Client_Send(ncsdk_GetClient(), ncsdk_Message_NewFromJson(use_json));
}
