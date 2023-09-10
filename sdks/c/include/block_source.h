/// @file block_source.h
/// @brief NovelCraft BlockSource interfaces

#pragma once

#ifdef __cplusplus
extern "C" {
#endif

#include "block.h"
#include "position.h"

/// @struct ncsdk_BlockSource
/// @brief Represents a collection of blocks.
struct ncsdk_BlockSource;
typedef struct ncsdk_BlockSource ncsdk_BlockSource;

/// @brief Gets the block at the position.
/// @param self The block source.
/// @param position The position.
/// @return The block, or NULL if not found.
const ncsdk_Block* ncsdk_BlockSource_GetBlock(const ncsdk_BlockSource* self,
                                         const ncsdk_Position(int) * position);

#ifdef __cplusplus
}
#endif
