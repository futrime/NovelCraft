#pragma once

#ifdef __cplusplus
extern "C" {
#endif

#include "dictionary.h"
#include "internal/block.h"
#include "internal/section.h"
#include "position.h"

struct ncsdk_BlockSource {
  ncsdk_Dictionary* section_dictionary;
};
typedef struct ncsdk_BlockSource ncsdk_BlockSource;

ncsdk_BlockSource* ncsdk_BlockSource_New();

void ncsdk_BlockSource_Delete(ncsdk_BlockSource* self);

void ncsdk_BlockSource_AddSection(ncsdk_BlockSource* self,
                                  const ncsdk_Section* section);

void ncsdk_BlockSource_Clear(ncsdk_BlockSource* self);

const ncsdk_Block* ncsdk_BlockSource_GetBlock(const ncsdk_BlockSource* self,
                                              const ncsdk_Position(int) *
                                                  position);

const ncsdk_Section* ncsdk_BlockSource_GetSection(const ncsdk_BlockSource* self,
                                                  const ncsdk_Position(int) *
                                                      position);

void ncsdk_BlockSource_RemoveSection(ncsdk_BlockSource* self,
                                     const ncsdk_Position(int) * position);

void ncsdk_BlockSource_SetBlock(ncsdk_BlockSource* self,
                                const ncsdk_Position(int) * position,
                                const ncsdk_Block* block);

#ifdef __cplusplus
}
#endif
