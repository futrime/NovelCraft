#include "array.h"

#include <stddef.h>
#include <stdlib.h>
#include <string.h>

struct ncsdk_Array {
  void* data;
  size_t size;
  size_t element_size;
};

ncsdk_Array* ncsdk_Array_NewWithElementSize(size_t element_size, size_t size) {
  ncsdk_Array* array = malloc(sizeof(ncsdk_Array));
  array->data = calloc(size, element_size);
  array->size = size;
  array->element_size = element_size;
  return array;
}

void ncsdk_Array_Delete(ncsdk_Array* self) {
  free(self->data);
  free(self);
}

void* ncsdk_Array_AtWithoutType(ncsdk_Array* self, size_t index) {
  if (index >= self->size) {
    return NULL;
  }
  return (char*)self->data + index * self->element_size;
}

void ncsdk_Array_Fill(ncsdk_Array* self, void* value) {
  for (size_t i = 0; i < self->size; ++i) {
    void* element = ncsdk_Array_AtWithoutType(self, i);
    memcpy(element, value, self->element_size);
  }
}

size_t ncsdk_Array_Size(ncsdk_Array* self) { return self->size; }
