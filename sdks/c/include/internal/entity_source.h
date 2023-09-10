#pragma once

#ifdef __cplusplus
extern "C" {
#endif

#include "dictionary.h"
#include "internal/entity.h"
#include "list.h"

struct ncsdk_EntitySource {
  ncsdk_Dictionary* entity_dictionary;
};
typedef struct ncsdk_EntitySource ncsdk_EntitySource;

ncsdk_EntitySource* ncsdk_EntitySource_New();

void ncsdk_EntitySource_Delete(ncsdk_EntitySource* self);

void ncsdk_EntitySource_AddEntity(ncsdk_EntitySource* self,
                                  const ncsdk_Entity* entity);

void ncsdk_EntitySource_Clear(ncsdk_EntitySource* self);

const ncsdk_List* ncsdk_EntitySource_GetAllEntities(ncsdk_EntitySource* self);

const ncsdk_Entity* ncsdk_EntitySource_GetEntity(
    const ncsdk_EntitySource* entity_source, int unique_id);

void ncsdk_EntitySource_RemoveEntity(ncsdk_EntitySource* self, int unique_id);

void ncsdk_EntitySource_SetEntity(ncsdk_EntitySource* self, int unique_id,
                                  const ncsdk_Entity* entity);

#ifdef __cplusplus
}
#endif
