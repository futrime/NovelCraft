#include "stack.h"

#include <stddef.h>
#include <stdlib.h>

#include "list.h"

struct ncsdk_Stack {
  ncsdk_List* list;
};

ncsdk_Stack* ncsdk_Stack_NewWithElementSize(size_t element_size) {
  ncsdk_Stack* stack = malloc(sizeof(ncsdk_Stack));
  stack->list = ncsdk_List_NewWithElementSize(element_size);
  return stack;
}

void ncsdk_Stack_Delete(ncsdk_Stack* self) {
  ncsdk_List_Delete(self->list);
  free(self);
}

void ncsdk_Stack_Pop(ncsdk_Stack* self) {
  ncsdk_List_Erase(self->list, ncsdk_List_Size(self->list) - 1);
}

void ncsdk_Stack_Push(ncsdk_Stack* self, const void* element) {
  ncsdk_List_PushBack(self->list, element);
}

size_t ncsdk_Stack_Size(ncsdk_Stack* self) {
  return ncsdk_List_Size(self->list);
}

void* ncsdk_Stack_TopWithoutType(ncsdk_Stack* self) {
  return ncsdk_List_At(void, self->list, ncsdk_List_Size(self->list) - 1);
}
