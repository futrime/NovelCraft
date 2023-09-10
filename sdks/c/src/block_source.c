#include "internal/block_source.h"

#include <stdlib.h>

#include "internal/section.h"
#include "position.h"

ncsdk_BlockSource* ncsdk_BlockSource_New() {
  ncsdk_BlockSource* self =
      (ncsdk_BlockSource*)malloc(sizeof(ncsdk_BlockSource));
  self->section_dictionary =
      ncsdk_Dictionary_New(ncsdk_Position(int), ncsdk_Section);
  return self;
}

void ncsdk_BlockSource_Delete(ncsdk_BlockSource* self) {
  ncsdk_Dictionary_Delete(self->section_dictionary);
  free(self);
}

void ncsdk_BlockSource_AddSection(ncsdk_BlockSource* self,
                                  const ncsdk_Section* section) {
  ncsdk_Dictionary_Insert(self->section_dictionary,
                          ncsdk_Section_GetPosition(section), section);
}

void ncsdk_BlockSource_Clear(ncsdk_BlockSource* self) {
  ncsdk_Dictionary_Clear(self->section_dictionary);
}

const ncsdk_Block* ncsdk_BlockSource_GetBlock(const ncsdk_BlockSource* self,
                                              const ncsdk_Position(int) *
                                                  position) {
  const ncsdk_Section* section = ncsdk_BlockSource_GetSection(self, position);
  if (section == NULL) {
    return NULL;
  }
  return ncsdk_Section_GetBlock(section, position);
}

const ncsdk_Section* ncsdk_BlockSource_GetSection(const ncsdk_BlockSource* self,
                                                  const ncsdk_Position(int) *
                                                      position) {
  return ncsdk_Dictionary_At(const ncsdk_Section, self->section_dictionary,
                             position);
}

void ncsdk_BlockSource_RemoveSection(ncsdk_BlockSource* self,
                                     const ncsdk_Position(int) * position) {
  ncsdk_Dictionary_Erase(self->section_dictionary, position);
}

void ncsdk_BlockSource_SetBlock(ncsdk_BlockSource* self,
                                const ncsdk_Position(int) * position,
                                const ncsdk_Block* block) {
  ncsdk_Section* section =
      (ncsdk_Section*)ncsdk_BlockSource_GetSection(self, position);
  if (section == NULL) {
    int block_id_array[16][16][16] = {0};
    section = ncsdk_Section_New(position, block_id_array);
    ncsdk_BlockSource_AddSection(self, section);
  }
  *(ncsdk_Block*)ncsdk_Section_GetBlock(section, position) = *block;
}
