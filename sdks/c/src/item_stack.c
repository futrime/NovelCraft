#include "internal/item_stack.h"

#include <stdlib.h>

struct ncsdk_ItemStack* ncsdk_ItemStack_New(int type_id, int count) {
  struct ncsdk_ItemStack* self = malloc(sizeof(struct ncsdk_ItemStack));
  self->type_id = type_id;
  self->count = count;
  return self;
}

void ncsdk_ItemStack_Delete(struct ncsdk_ItemStack* self) { free(self); }

int ncsdk_ItemStack_GetCount(const struct ncsdk_ItemStack* self) {
  return self->count;
}

int ncsdk_ItemStack_GetTypeId(const struct ncsdk_ItemStack* self) {
  return self->type_id;
}
