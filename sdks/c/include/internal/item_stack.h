#pragma once

#ifdef __cplusplus
extern "C" {
#endif

struct ncsdk_ItemStack {
  int type_id;
  int count;
};
typedef struct ncsdk_ItemStack ncsdk_ItemStack;

ncsdk_ItemStack* ncsdk_ItemStack_New(int type_id, int count);

void ncsdk_ItemStack_Delete(ncsdk_ItemStack* self);

int ncsdk_ItemStack_GetCount(const ncsdk_ItemStack* self);

int ncsdk_ItemStack_GetTypeId(const ncsdk_ItemStack* self);

#ifdef __cplusplus
}
#endif
