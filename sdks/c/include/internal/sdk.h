#pragma once

#ifdef __cplusplus
extern "C" {
#endif

#include "array.h"
#include "dictionary.h"
#include "internal/agent.h"
#include "internal/block.h"
#include "internal/block_source.h"
#include "internal/client.h"
#include "internal/entity.h"
#include "internal/entity_source.h"
#include "internal/inventory.h"
#include "internal/item_stack.h"
#include "internal/logger.h"
#include "internal/message.h"
#include "internal/section.h"
#include "list.h"
#include "optional.h"
#include "orientation.h"
#include "position.h"
#include "queue.h"
#include "stack.h"

void ncsdk_Initialize(int argc, char *argv[]);

void ncsdk_Finalize();

ncsdk_Agent *ncsdk_GetAgent();

const ncsdk_BlockSource *ncsdk_GetBlocks();

ncsdk_Client *ncsdk_GetClient();

const ncsdk_EntitySource *ncsdk_GetEntities();

ncsdk_Optional(float) ncsdk_GetLatency();

const ncsdk_Logger *ncsdk_GetLogger();

ncsdk_Optional(int) ncsdk_GetTick();

ncsdk_Optional(float) ncsdk_GetTicksPerSecond();

void ncsdk_Refresh();

#ifdef __cplusplus
}
#endif
