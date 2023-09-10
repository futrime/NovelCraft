#include <gtest/gtest.h>
#include <stack.h>
#include <stdlib.h>

TEST(StackTest, TestsValue) {
  ncsdk_Stack* stack = ncsdk_Stack_New(int);
  EXPECT_NE(stack, nullptr);

  for (size_t i = 0; i < 10; ++i) {
    int v = (int)i;
    ncsdk_Stack_Push(stack, &v);
    EXPECT_EQ(ncsdk_Stack_Size(stack), i + 1);
  }

  for (size_t i = 0; i < 10; ++i) {
    ncsdk_Stack_Pop(stack);
    EXPECT_EQ(ncsdk_Stack_Size(stack), 9 - i);
  }

  ncsdk_Stack_Delete(stack);
  }

TEST(StackTest, TestsPointer) {
  ncsdk_Stack* stack = ncsdk_Stack_New(char*);
  EXPECT_NE(stack, nullptr);

  for (size_t i = 0; i < 10; ++i) {
    char* v = new char[10];
    ncsdk_Stack_Push(stack, &v);
    EXPECT_EQ(ncsdk_Stack_Size(stack), i + 1);
  }

  for (size_t i = 0; i < 10; ++i) {
    ncsdk_Stack_Pop(stack);
    EXPECT_EQ(ncsdk_Stack_Size(stack), 9 - i);
  }

  ncsdk_Stack_Delete(stack);
  }

TEST(StackTest, TestsTop) {
  ncsdk_Stack* stack = ncsdk_Stack_New(int);
  EXPECT_NE(stack, nullptr);

  for (size_t i = 0; i < 10; ++i) {
    int v = (int)i;
    ncsdk_Stack_Push(stack, &v);
    EXPECT_EQ(ncsdk_Stack_Size(stack), i + 1);
  }

  for (size_t i = 0; i < 10; ++i) {
    int* v = ncsdk_Stack_Top(int, stack);
    EXPECT_EQ(*v, 9 - i);

    ncsdk_Stack_Pop(stack);
  }
}

TEST(StackTest, HandlesRandomPushAndPopWithoutCheck) {
  ncsdk_Stack* stack = ncsdk_Stack_New(int);
  EXPECT_NE(stack, nullptr);

  size_t size = 0;
  for (size_t i = 0; i < 100000; ++i) {
    int v = (int)i;
    if (rand() % 2 == 0) {
      ncsdk_Stack_Push(stack, &v);
      ++size;
      EXPECT_EQ(ncsdk_Stack_Size(stack), size);
    } else {
      if (size > 0) {
        ncsdk_Stack_Pop(stack);
        size = size > 0 ? size - 1 : 0;
        EXPECT_EQ(ncsdk_Stack_Size(stack), size);
      }
    }
  }
}
