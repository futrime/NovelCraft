#include "internal/entity_source.h"

#include <stdlib.h>

#include "dictionary.h"
#include "internal/entity.h"
#include "list.h"

ncsdk_EntitySource* ncsdk_EntitySource_New() {
  ncsdk_EntitySource* entity_source =
      (ncsdk_EntitySource*)malloc(sizeof(ncsdk_EntitySource));
  entity_source->entity_dictionary = ncsdk_Dictionary_New(int, ncsdk_Entity);
  return entity_source;
}

void ncsdk_EntitySource_Delete(ncsdk_EntitySource* entity_source) {
  ncsdk_Dictionary_Delete(entity_source->entity_dictionary);
  free(entity_source);
}

void ncsdk_EntitySource_AddEntity(ncsdk_EntitySource* self,
                                  const ncsdk_Entity* entity) {
  int unique_id = ncsdk_Entity_GetUniqueId(entity);
  ncsdk_Dictionary_Insert(self->entity_dictionary, &unique_id, entity);
}

void ncsdk_EntitySource_Clear(ncsdk_EntitySource* self) {
  ncsdk_Dictionary_Clear(self->entity_dictionary);
}

const ncsdk_List* ncsdk_EntitySource_GetAllEntities(ncsdk_EntitySource* self) {
  return ncsdk_Dictionary_GetValues(self->entity_dictionary);
}

const ncsdk_Entity* ncsdk_EntitySource_GetEntity(
    const ncsdk_EntitySource* entity_source, int unique_id) {
  return ncsdk_Dictionary_At(const ncsdk_Entity,
                             entity_source->entity_dictionary, &unique_id);
}

void ncsdk_EntitySource_RemoveEntity(ncsdk_EntitySource* self, int unique_id) {
  ncsdk_Dictionary_Erase(self->entity_dictionary, &unique_id);
}

void ncsdk_EntitySource_SetEntity(ncsdk_EntitySource* self, int unique_id,
                                  const ncsdk_Entity* entity) {
  ncsdk_Dictionary_Insert(self->entity_dictionary, &unique_id, entity);
}
