#include "internal/block.h"

#include <stdlib.h>

#include "position.h"

ncsdk_Block* ncsdk_Block_New(int type_id,
                             const ncsdk_Position(int) * position) {
  ncsdk_Block* self = (ncsdk_Block*)malloc(sizeof(ncsdk_Block));
  self->position = *position;
  self->type_id = type_id;
  return self;
}

void ncsdk_Block_Delete(ncsdk_Block* self) { free(self); }

const ncsdk_Position(int) * ncsdk_Block_GetPosition(const ncsdk_Block* self) {
  return &self->position;
}

int ncsdk_Block_GetTypeId(const ncsdk_Block* self) { return self->type_id; }
