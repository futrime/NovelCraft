#pragma once

#ifdef __cplusplus
extern "C" {
#endif

#include "position.h"

struct ncsdk_Block {
  ncsdk_Position(int) position;
  int type_id;
};
typedef struct ncsdk_Block ncsdk_Block;

ncsdk_Block* ncsdk_Block_New(int type_id, const ncsdk_Position(int) * position);

void ncsdk_Block_Delete(ncsdk_Block* self);

const ncsdk_Position(int) * ncsdk_Block_GetPosition(const ncsdk_Block* self);

int ncsdk_Block_GetTypeId(const ncsdk_Block* self);

#ifdef __cplusplus
}
#endif
