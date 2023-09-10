

#include "internal/inventory.h"

#include <jansson.h>
#include <stdlib.h>

#include "array.h"
#include "internal/client.h"
#include "internal/item_stack.h"
#include "internal/message.h"
#include "internal/sdk.h"

#define SIZE 36
#define HOTBAR_SIZE 9

struct ncsdk_Inventory* ncsdk_Inventory_New() {
  struct ncsdk_Inventory* self = malloc(sizeof(struct ncsdk_Inventory));
  self->item_stack_array = ncsdk_Array_New(struct ncsdk_ItemStack, SIZE);
  self->size = SIZE;
  self->hot_bar_size = HOTBAR_SIZE;
  self->main_hand_slot = 0;
  return self;
}

void ncsdk_Inventory_Delete(struct ncsdk_Inventory* self) {
  ncsdk_Array_Delete(self->item_stack_array);
  free(self);
}

const struct ncsdk_ItemStack* ncsdk_Inventory_Get(
    const struct ncsdk_Inventory* self, int slot) {
  return ncsdk_Array_At(struct ncsdk_ItemStack, self->item_stack_array, slot);
}

size_t ncsdk_Inventory_GetHotBarSize(const struct ncsdk_Inventory* self) {
  return self->hot_bar_size;
}

size_t ncsdk_Inventory_GetMainHandSlot(const struct ncsdk_Inventory* self) {
  return self->main_hand_slot;
}

void ncsdk_Inventory_SetMainHandSlot(struct ncsdk_Inventory* self, int slot) {
  self->main_hand_slot = slot;
  // TODO: Send packet to server.
  // Construct the attack message
  json_t* set_main_hand_slot_json = json_object();
  json_object_set(set_main_hand_slot_json, "bound_to",
                  ncsdk_Message_BoundToKind_ServerBound);
  json_object_set(set_main_hand_slot_json, "type",
                  ncsdk_Message_MessageKind_PerformSwitchMainHandSlot);
  json_object_set(set_main_hand_slot_json, "token", ncsdk_Agent_GetToken(self));
  json_object_set(set_main_hand_slot_json, "new_main_hand", json_integer(slot));
  // Send it to server
  ncsdk_Client_Send(ncsdk_GetClient(),
                    ncsdk_Message_NewFromJson(set_main_hand_slot_json));
}

size_t ncsdk_Inventory_GetSize(const struct ncsdk_Inventory* self) {
  return self->size;
}

void ncsdk_Inventory_DropItem(struct ncsdk_Inventory* self, int slot,
                              int count) {
  // TODO: Send packet to server.
  // ?? The items should be an array accrording to the definition in the server
  // Construct the attack message
  json_t* drop_item_json = json_object();
  json_object_set(drop_item_json, "bound_to",
                  ncsdk_Message_BoundToKind_ServerBound);
  json_object_set(drop_item_json, "type",
                  ncsdk_Message_MessageKind_PerformDropItem);
  json_object_set(drop_item_json, "token", ncsdk_Agent_GetToken(self));
  json_object_set(drop_item_json, "drop_items", json_integer(slot));
  // Send it to server
  ncsdk_Client_Send(ncsdk_GetClient(),
                    ncsdk_Message_NewFromJson(drop_item_json));
}

void ncsdk_Inventory_MergeSlots(struct ncsdk_Inventory* self, int from_slot,
                                int to_slot) {
  // TODO: Send packet to server.
  // Construct the attack message
  json_t* merge_slots_json = json_object();
  json_object_set(merge_slots_json, "bound_to",
                  ncsdk_Message_BoundToKind_ServerBound);
  json_object_set(merge_slots_json, "type",
                  ncsdk_Message_MessageKind_PerformMergeSlots);
  json_object_set(merge_slots_json, "token", ncsdk_Agent_GetToken(self));
  json_object_set(merge_slots_json, "from_slot", json_integer(from_slot));
  json_object_set(merge_slots_json, "to_slot", json_integer(to_slot));
  // Send it to server
  ncsdk_Client_Send(ncsdk_GetClient(),
                    ncsdk_Message_NewFromJson(merge_slots_json));
}

void ncsdk_Inventory_SwapSlots(struct ncsdk_Inventory* self, int slot1,
                               int slot2) {
  // TODO: Send packet to server.
  // Construct the attack message
  json_t* swap_slots_json = json_object();
  json_object_set(swap_slots_json, "bound_to",
                  ncsdk_Message_BoundToKind_ServerBound);
  json_object_set(swap_slots_json, "type",
                  ncsdk_Message_MessageKind_PerformSwapSlots);
  json_object_set(swap_slots_json, "token", ncsdk_Agent_GetToken(self));
  json_object_set(swap_slots_json, "slot_a", json_integer(slot1));
  json_object_set(swap_slots_json, "slot_b", json_integer(slot2));
  // Send it to server
  ncsdk_Client_Send(ncsdk_GetClient(),
                    ncsdk_Message_NewFromJson(swap_slots_json));
}
