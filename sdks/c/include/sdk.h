/// @file sdk.h
/// @brief NovelCraft SDK interfaces

#pragma once

#ifdef __cplusplus
extern "C" {
#endif

#include "agent.h"
#include "array.h"
#include "block.h"
#include "block_source.h"
#include "dictionary.h"
#include "entity.h"
#include "entity_source.h"
#include "inventory.h"
#include "item_stack.h"
#include "list.h"
#include "logger.h"
#include "optional.h"
#include "orientation.h"
#include "position.h"
#include "queue.h"
#include "stack.h"

/// @brief Initializes the SDK.
/// @param argc The number of arguments.
void ncsdk_Initialize(int argc, char *argv[]);

/// @brief Finalizes the SDK.
void ncsdk_Finalize();

/// @brief Gets the agent representing the player controlled by the user.
/// @return The agent.
ncsdk_Agent *ncsdk_GetAgent();

/// @brief Gets the block collection.
/// @return The block collection.
const ncsdk_BlockSource *ncsdk_GetBlocks();

/// @brief Gets the entity collection.
/// @return The entity collection.
const ncsdk_EntitySource *ncsdk_GetEntities();

/// @brief Gets the logger.
/// @return The logger.
const ncsdk_Logger *ncsdk_GetLogger();

/// @brief Gets the current tick.
/// @return The current tick. -1 if no tick information is received.
ncsdk_Optional(int) ncsdk_GetTick();

/// @brief Gets the number of ticks per second.
/// @return The number of ticks per second. -1.0 if no tick information is
/// received.
ncsdk_Optional(float) ncsdk_GetTicksPerSecond();

/// @brief Refreshes the SDK.
/// @note This function should be called periodically to update the SDK.
void ncsdk_Refresh();

#ifdef __cplusplus
}
#endif
