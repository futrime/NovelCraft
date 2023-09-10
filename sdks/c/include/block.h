/// @file block.h
/// @brief NovelCraft Block interfaces

#pragma once

#ifdef __cplusplus
extern "C" {
#endif

#include "position.h"

/// @struct ncsdk_Block
/// @brief Represents a block.
struct ncsdk_Block;
typedef struct ncsdk_Block ncsdk_Block;

/// @brief Gets the block's position
/// @param self The block.
/// @return The block's position.
const ncsdk_Position(int)* ncsdk_Block_GetPosition(
    const ncsdk_Block* self);

/// @brief Gets the block's type ID.
/// @param self The block.
/// @return The block's type ID.
int ncsdk_Block_GetTypeId(const ncsdk_Block* self);

#ifdef __cplusplus
}
#endif
