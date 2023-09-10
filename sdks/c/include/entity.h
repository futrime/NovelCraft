/// @file entity.h
/// @brief NovelCraft Entity interfaces

#pragma once

#ifdef __cplusplus
extern "C" {
#endif

#include "orientation.h"
#include "position.h"

/// @struct ncsdk_Entity
/// @brief Represents an entity.
struct ncsdk_Entity;
typedef struct ncsdk_Entity ncsdk_Entity;

/// @brief Gets the entity's orientation.
/// @param entity The entity.
/// @return The entity's orientation.
const ncsdk_Orientation* ncsdk_Entity_GetOrientation(
    const ncsdk_Entity* entity);

/// @brief Gets the entity's position.
/// @param entity The entity.
/// @return The entity's position.
const ncsdk_Position(float) *
    ncsdk_Entity_GetPosition(const ncsdk_Entity* entity);

/// @brief Gets the entity's type ID.
/// @param entity The entity.
/// @return The entity's type ID.
int ncsdk_Entity_GetTypeId(const ncsdk_Entity* entity);

/// @brief Gets the entity's unique ID.
/// @param entity The entity.
/// @return The entity's unique ID.
int ncsdk_Entity_GetUniqueId(const ncsdk_Entity* entity);

#ifdef __cplusplus
}
#endif
