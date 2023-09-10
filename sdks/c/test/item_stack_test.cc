#include <gtest/gtest.h>
#include <internal/item_stack.h>
#include <item_stack.h>

TEST(ItemStackTest, Tests) {
  struct ncsdk_ItemStack* item_stack = ncsdk_ItemStack_New(1, 10);
  EXPECT_NE(item_stack, nullptr);

  EXPECT_EQ(ncsdk_ItemStack_GetTypeId(item_stack), 1);
  EXPECT_EQ(ncsdk_ItemStack_GetCount(item_stack), 10);

  ncsdk_ItemStack_Delete(item_stack);
}
