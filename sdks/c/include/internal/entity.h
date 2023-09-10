#pragma once

#ifdef __cplusplus
extern "C" {
#endif

#include "orientation.h"
#include "position.h"

struct ncsdk_Entity {
  ncsdk_Orientation orientation;
  ncsdk_Position(float) position;
  int type_id;
  int unique_id;
};
typedef struct ncsdk_Entity ncsdk_Entity;

ncsdk_Entity* ncsdk_Entity_New(ncsdk_Orientation orientation,
                               ncsdk_Position(float) position, int type_id,
                               int unique_id);

void ncsdk_Entity_Delete(ncsdk_Entity* self);

ncsdk_Orientation ncsdk_Entity_GetOrientation(const ncsdk_Entity* self);

ncsdk_Position(float) ncsdk_Entity_GetPosition(const ncsdk_Entity* self);

int ncsdk_Entity_GetTypeId(const ncsdk_Entity* self);

int ncsdk_Entity_GetUniqueId(const ncsdk_Entity* self);

#ifdef __cplusplus
}
#endif
