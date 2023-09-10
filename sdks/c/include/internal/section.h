#pragma once

#ifdef __cplusplus
extern "C" {
#endif

#include "block.h"
#include "position.h"

struct ncsdk_Section {
  ncsdk_Position(int) position;
  int block_id_array[16][16][16];
};
typedef struct ncsdk_Section ncsdk_Section;

ncsdk_Section* ncsdk_Section_New(const ncsdk_Position(int) * position,
                                 int block_id_array[16][16][16]);

void ncsdk_Section_Delete(ncsdk_Section* self);

const ncsdk_Block* ncsdk_Section_GetBlock(const ncsdk_Section* self,
                                          const ncsdk_Position(int) *
                                              relative_position);

const ncsdk_Position(int) *
    ncsdk_Section_GetPosition(const ncsdk_Section* self);

void ncsdk_Section_SetBlock(ncsdk_Section* self,
                            const ncsdk_Position(int) * relative_position,
                            const ncsdk_Block* block);

#ifdef __cplusplus
}
#endif
