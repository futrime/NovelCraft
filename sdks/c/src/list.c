#include "list.h"

#include <stddef.h>
#include <stdlib.h>
#include <string.h>

#include "array.h"

struct ncsdk_List {
  ncsdk_Array* array;
  size_t size;
  size_t element_size;
};

ncsdk_List* ncsdk_List_NewWithElementSize(size_t element_size) {
  ncsdk_List* list = malloc(sizeof(ncsdk_List));
  list->array = ncsdk_Array_NewWithElementSize(element_size, 1);
  list->size = 0;
  list->element_size = element_size;
  return list;
}

void ncsdk_List_Delete(ncsdk_List* self) {
  ncsdk_Array_Delete(self->array);
  free(self);
}

void* ncsdk_List_AtWithoutType(ncsdk_List* self, size_t index) {
  if (index >= self->size) {
    return NULL;
  }

  return ncsdk_Array_AtWithoutType(self->array, index);
}

void ncsdk_List_Clear(ncsdk_List* self) { self->size = 0; }

void ncsdk_List_Erase(ncsdk_List* self, size_t index) {
  if (index >= self->size) {
    return;
  }

  memmove(ncsdk_Array_AtWithoutType(self->array, index),
          ncsdk_Array_AtWithoutType(self->array, index + 1),
          (self->size - index - 1) * self->element_size);
  --self->size;
}

void ncsdk_List_Fill(ncsdk_List* self, void* value) {
  ncsdk_Array_Fill(self->array, value);
}

void ncsdk_List_Insert(ncsdk_List* self, size_t index, const void* element) {
  if (index > self->size) {
    return;
  }

  if (self->size >= ncsdk_Array_Size(self->array)) {
    ncsdk_Array* new_array = ncsdk_Array_NewWithElementSize(
        self->element_size, ncsdk_Array_Size(self->array) * 2);
    memcpy(ncsdk_Array_AtWithoutType(new_array, 0),
           ncsdk_Array_AtWithoutType(self->array, 0),
           ncsdk_Array_Size(self->array) * self->element_size);
    ncsdk_Array_Delete(self->array);
    self->array = new_array;
  }

  memmove(ncsdk_Array_AtWithoutType(self->array, index + 1),
          ncsdk_Array_AtWithoutType(self->array, index),
          (self->size - index) * self->element_size);

  void* dest = ncsdk_Array_AtWithoutType(self->array, index);
  memcpy(dest, element, self->element_size);
  ++self->size;
}

void ncsdk_List_PopBack(ncsdk_List* self) {
  self->size = self->size > 0 ? self->size - 1 : 0;
}

void ncsdk_List_PushBack(ncsdk_List* self, const void* element) {
  if (self->size >= ncsdk_Array_Size(self->array)) {
    ncsdk_Array* new_array = ncsdk_Array_NewWithElementSize(
        self->element_size, ncsdk_Array_Size(self->array) * 2);
    memcpy(ncsdk_Array_AtWithoutType(new_array, 0),
           ncsdk_Array_AtWithoutType(self->array, 0),
           ncsdk_Array_Size(self->array) * self->element_size);
    ncsdk_Array_Delete(self->array);
    self->array = new_array;
  }

  void* dest = ncsdk_Array_AtWithoutType(self->array, self->size);
  memcpy(dest, element, self->element_size);
  ++self->size;
}

size_t ncsdk_List_Size(ncsdk_List* self) { return self->size; }
