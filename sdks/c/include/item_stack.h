/// @file item_stack.h
/// @brief NovelCraft ItemStack interfaces

#pragma once

#ifdef __cplusplus
extern "C" {
#endif

/// @struct ncsdk_ItemStack
/// @brief Represents a stack of items.
struct ncsdk_ItemStack;
typedef struct ncsdk_ItemStack ncsdk_ItemStack;

/// @brief Gets the count of items in the stack.
/// @param self The item stack.
/// @return The count of items in the stack.
int ncsdk_ItemStack_GetCount(const ncsdk_ItemStack* self);

/// @brief Gets the type ID of the items.
/// @param self The item stack.
/// @return The type ID.
int ncsdk_ItemStack_GetTypeId(const ncsdk_ItemStack* self);

#ifdef __cplusplus
}
#endif
