#include "internal/entity.h"

#include <stdlib.h>

#include "orientation.h"
#include "position.h"

ncsdk_Entity *ncsdk_Entity_New(ncsdk_Orientation orientation,
                               ncsdk_Position(float) position, int type_id,
                               int unique_id) {
  ncsdk_Entity *entity = malloc(sizeof(ncsdk_Entity));
  entity->orientation = orientation;
  entity->position = position;
  entity->type_id = type_id;
  entity->unique_id = unique_id;
  return entity;
}

void ncsdk_Entity_Delete(ncsdk_Entity *entity) { free(entity); }

ncsdk_Orientation ncsdk_Entity_GetOrientation(const ncsdk_Entity *entity) {
  return entity->orientation;
}

ncsdk_Position(float) ncsdk_Entity_GetPosition(const ncsdk_Entity *entity) {
  return entity->position;
}

int ncsdk_Entity_GetTypeId(const ncsdk_Entity *entity) {
  return entity->type_id;
}

int ncsdk_Entity_GetUniqueId(const ncsdk_Entity *entity) {
  return entity->unique_id;
}
