#include "dictionary.h"

#include <stdbool.h>
#include <stdlib.h>
#include <string.h>

#include "list.h"

struct ncsdk_Dictionary {
  ncsdk_List* key_list;
  size_t key_element_size;
  ncsdk_List* value_list;
  size_t value_element_size;
};

ncsdk_Dictionary* ncsdk_Dictionary_NewWithElementSizes(size_t key_size,
                                                       size_t value_size) {
  ncsdk_Dictionary* dictionary = malloc(sizeof(ncsdk_Dictionary));
  dictionary->key_list = ncsdk_List_NewWithElementSize(key_size);
  dictionary->key_element_size = key_size;
  dictionary->value_list = ncsdk_List_NewWithElementSize(value_size);
  dictionary->value_element_size = value_size;
  return dictionary;
}

void ncsdk_Dictionary_Delete(ncsdk_Dictionary* self) {
  ncsdk_List_Delete(self->key_list);
  ncsdk_List_Delete(self->value_list);
  free(self);
}

void* ncsdk_Dictionary_AtWithoutType(ncsdk_Dictionary* self, const void* key) {
  for (size_t i = 0; i < ncsdk_List_Size(self->key_list); ++i) {
    if (memcmp(ncsdk_List_At(void, self->key_list, i), key,
               self->key_element_size) == 0) {
      return ncsdk_List_At(void, self->value_list, i);
    }
  }
  return NULL;
}

void ncsdk_Dictionary_Clear(ncsdk_Dictionary* self) {
  ncsdk_List_Clear(self->key_list);
  ncsdk_List_Clear(self->value_list);
}

bool ncsdk_Dictionary_Contains(ncsdk_Dictionary* self, const void* key) {
  return ncsdk_Dictionary_AtWithoutType(self, key) != NULL;
}

void ncsdk_Dictionary_Erase(ncsdk_Dictionary* self, const void* key) {
  for (size_t i = 0; i < ncsdk_List_Size(self->key_list); ++i) {
    if (memcmp(ncsdk_List_At(void, self->key_list, i), key,
               self->key_element_size) == 0) {
      ncsdk_List_Erase(self->key_list, i);
      ncsdk_List_Erase(self->value_list, i);
      return;
    }
  }
}

void ncsdk_Dictionary_Insert(ncsdk_Dictionary* self, const void* key,
                             const void* value) {
  if (ncsdk_Dictionary_Contains(self, key)) {
    ncsdk_Dictionary_Erase(self, key);
  }

  ncsdk_List_PushBack(self->key_list, key);
  ncsdk_List_PushBack(self->value_list, value);
}

const ncsdk_List* ncsdk_Dictionary_GetKeys(ncsdk_Dictionary* self) {
  return self->key_list;
}

const ncsdk_List* ncsdk_Dictionary_GetValues(ncsdk_Dictionary* self) {
  return self->value_list;
}

size_t ncsdk_Dictionary_Size(ncsdk_Dictionary* self) {
  return ncsdk_List_Size(self->key_list);
}
