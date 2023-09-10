/// @file inventory.h
/// @brief NovelCraft Inventory interfaces

#pragma once

#ifdef __cplusplus
extern "C" {
#endif

#include <stddef.h>

#include "item_stack.h"

/// @struct ncsdk_Inventory
/// @brief Represents an inventory.
struct ncsdk_Inventory;
typedef struct ncsdk_Inventory ncsdk_Inventory;

/// @brief Gets the item in a slot.
/// @param self The inventory.
/// @param slot The slot.
/// @return The item in the slot. Or NULL if the slot is empty.
const ncsdk_ItemStack* ncsdk_Inventory_Get(
    const ncsdk_Inventory* self, int slot);

/// @brief Gets the number of slots in the hot bar.
/// @param self The inventory.
/// @return The number of slots in the hot bar.
size_t ncsdk_Inventory_GetHotBarSize(const ncsdk_Inventory* self);

/// @brief Gets the slot that the main hand is currently in.
/// @param self The inventory.
/// @return The slot that the main hand is currently in.
size_t ncsdk_Inventory_GetMainHandSlot(const ncsdk_Inventory* self);

/// @brief Sets the slot that the main hand is currently in.
/// @param self The inventory.
/// @param slot The slot that the main hand is currently in.
void ncsdk_Inventory_SetMainHandSlot(ncsdk_Inventory* self, int slot);

/// @brief Gets the number of slots in the inventory.
/// @param self The inventory.
/// @return The number of slots in the inventory.
size_t ncsdk_Inventory_GetSize(const ncsdk_Inventory* self);

/// @brief Drops items from a slot.
/// @param self The inventory.
/// @param slot The slot.
/// @param count The number of items to drop.
void ncsdk_Inventory_DropItem(ncsdk_Inventory* self, int slot,
                              int count);

/// @brief Merges items from two slots into one slot.
/// @param self The inventory.
/// @param from_slot The slot to merge from.
/// @param to_slot The slot to merge to.
/// @note If the items in the two slots are not the same type, nothing happens.
/// If the number of items in the two slots is greater than the maximum stack
/// size, toSlot will be filled to the maximum stack size and the remaining
/// items will be left in fromSlot.
void ncsdk_Inventory_MergeSlots(ncsdk_Inventory* self, int from_slot,
                                int to_slot);

/// @brief Swaps items between two slots.
/// @param self The inventory.
/// @param slot1 The first slot.
/// @param slot2 The second slot.
void ncsdk_Inventory_SwapSlots(ncsdk_Inventory* self, int slot1,
                               int slot2);

#ifdef __cplusplus
}
#endif
