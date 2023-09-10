#include "queue.h"

#include <stddef.h>
#include <stdlib.h>

#include "list.h"

struct ncsdk_Queue {
  ncsdk_List* list;
};

ncsdk_Queue* ncsdk_Queue_NewWithElementSize(size_t element_size) {
  ncsdk_Queue* queue = malloc(sizeof(ncsdk_Queue));
  queue->list = ncsdk_List_NewWithElementSize(element_size);
  return queue;
}

void ncsdk_Queue_Delete(ncsdk_Queue* self) {
  ncsdk_List_Delete(self->list);
  free(self);
}

void* ncsdk_Queue_BackWithoutType(ncsdk_Queue* self) {
  return ncsdk_List_At(void, self->list, ncsdk_List_Size(self->list) - 1);
}

void* ncsdk_Queue_FrontWithoutType(ncsdk_Queue* self) {
  return ncsdk_List_At(void, self->list, 0);
}

void ncsdk_Queue_Pop(ncsdk_Queue* self) { ncsdk_List_Erase(self->list, 0); }

void ncsdk_Queue_Push(ncsdk_Queue* self, const void* element) {
  ncsdk_List_PushBack(self->list, element);
}

size_t ncsdk_Queue_Size(ncsdk_Queue* self) {
  return ncsdk_List_Size(self->list);
}
