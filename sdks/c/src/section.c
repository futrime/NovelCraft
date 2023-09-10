#include "internal/section.h"

#include <stdlib.h>
#include <string.h>

#include "block.h"
#include "position.h"

ncsdk_Section* ncsdk_Section_New(const ncsdk_Position(int) * position,
                                 int block_id_array[16][16][16]) {
  ncsdk_Section* self = (ncsdk_Section*)malloc(sizeof(ncsdk_Section));
  self->position = *position;
  memcpy(self->block_id_array, block_id_array, sizeof(self->block_id_array));
  return self;
}

void ncsdk_Section_Delete(ncsdk_Section* self) { free(self); }

const ncsdk_Block* ncsdk_Section_GetBlock(const ncsdk_Section* self,
                                          const ncsdk_Position(int) *
                                              relative_position) {
  if (relative_position->x < 0 || relative_position->x >= 16 ||
      relative_position->y < 0 || relative_position->y >= 16 ||
      relative_position->z < 0 || relative_position->z >= 16) {
    return NULL;
  }

  const ncsdk_Position(int)* section_position = ncsdk_Section_GetPosition(self);
  ncsdk_Position(int) absolute_position = {
      section_position->x + relative_position->x,
      section_position->y + relative_position->y,
      section_position->z + relative_position->z,
  };
  return ncsdk_Block_New(
      self->block_id_array[relative_position->x][relative_position->y]
                          [relative_position->z],
      &absolute_position);
}

const ncsdk_Position(int) *
    ncsdk_Section_GetPosition(const ncsdk_Section* self) {
  return &self->position;
}

void ncsdk_Section_SetBlock(ncsdk_Section* self,
                            const ncsdk_Position(int) * relative_position,
                            const ncsdk_Block* block) {
  if (relative_position->x < 0 || relative_position->x >= 16 ||
      relative_position->y < 0 || relative_position->y >= 16 ||
      relative_position->z < 0 || relative_position->z >= 16) {
    return;
  }

  self->block_id_array[relative_position->x][relative_position->y]
                      [relative_position->z] = ncsdk_Block_GetTypeId(block);
}
