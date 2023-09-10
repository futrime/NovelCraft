#include <gtest/gtest.h>
#include <list.h>
#include <stdlib.h>

TEST(ListTest, TestsValue) {
  ncsdk_List* list = ncsdk_List_New(int);
  EXPECT_NE(list, nullptr);

  for (int i = 0; i < 10; ++i) {
    ncsdk_List_PushBack(list, &i);
  }

  EXPECT_EQ(ncsdk_List_Size(list), 10);

  for (int i = 0; i < 10; ++i) {
    int* value = ncsdk_List_At(int, list, i);
    EXPECT_EQ(*value, i);
  }

  ncsdk_List_Delete(list);
  }

TEST(ListTest, TestsPointer) {
  ncsdk_List* list = ncsdk_List_New(int*);
  EXPECT_NE(list, nullptr);

  for (int i = 0; i < 10; ++i) {
    int* value = new int(i);
    ncsdk_List_PushBack(list, &value);
  }

  EXPECT_EQ(ncsdk_List_Size(list), 10);

  for (int i = 0; i < 10; ++i) {
    int** value = ncsdk_List_At(int*, list, i);
    EXPECT_EQ(**value, i);
    delete *value;
  }

  ncsdk_List_Delete(list);
  }

TEST(ListTest, TestsClearing) {
  ncsdk_List* list = ncsdk_List_New(int);
  EXPECT_NE(list, nullptr);

  for (int i = 0; i < 10; ++i) {
    ncsdk_List_PushBack(list, &i);
  }

  EXPECT_EQ(ncsdk_List_Size(list), 10);

  ncsdk_List_Clear(list);

  EXPECT_EQ(ncsdk_List_Size(list), 0);

  ncsdk_List_Delete(list);
  }

TEST(ListTest, TestsErasure) {
  ncsdk_List* list = ncsdk_List_New(int);
  EXPECT_NE(list, nullptr);

  for (int i = 0; i < 10; ++i) {
    ncsdk_List_PushBack(list, &i);
  }

  EXPECT_EQ(ncsdk_List_Size(list), 10);

  for (int i = 0; i < 10; ++i) {
    ncsdk_List_Erase(list, 0);
    EXPECT_EQ(ncsdk_List_Size(list), 9 - i);
    for (int j = 0; j < 9 - i; ++j) {
      int* value = ncsdk_List_At(int, list, j);
      EXPECT_EQ(*value, j + i + 1);
    }
  }

  ncsdk_List_Delete(list);
  }

TEST(ListTest, TestsFilling) {
  ncsdk_List* list = ncsdk_List_New(int);
  EXPECT_NE(list, nullptr);

  for (int i = 0; i < 10; ++i) {
    ncsdk_List_PushBack(list, &i);
  }

  EXPECT_EQ(ncsdk_List_Size(list), 10);

  int value = 0;
  ncsdk_List_Fill(list, &value);

  for (int i = 0; i < 10; ++i) {
    int* v = ncsdk_List_At(int, list, i);
    EXPECT_EQ(*v, 0);
  }

  ncsdk_List_Delete(list);
  }

TEST(ListTest, TestsInsertion) {
  ncsdk_List* list = ncsdk_List_New(int);
  EXPECT_NE(list, nullptr);

  for (int i = 0; i < 10; ++i) {
    ncsdk_List_PushBack(list, &i);
  }

  EXPECT_EQ(ncsdk_List_Size(list), 10);

  int value = 0;
  ncsdk_List_Insert(list, 0, &value);
  EXPECT_EQ(ncsdk_List_Size(list), 11);
  int* v = ncsdk_List_At(int, list, 0);
  EXPECT_EQ(*v, 0);

  value = 1;
  ncsdk_List_Insert(list, 5, &value);
  EXPECT_EQ(ncsdk_List_Size(list), 12);
  v = ncsdk_List_At(int, list, 5);
  EXPECT_EQ(*v, 1);

  value = 2;
  ncsdk_List_Insert(list, 11, &value);
  EXPECT_EQ(ncsdk_List_Size(list), 13);
  v = ncsdk_List_At(int, list, 11);
  EXPECT_EQ(*v, 2);

  ncsdk_List_Delete(list);
  }

TEST(ListTest, TestsPopBack) {
  ncsdk_List* list = ncsdk_List_New(int);
  EXPECT_NE(list, nullptr);

  for (int i = 0; i < 10; ++i) {
    ncsdk_List_PushBack(list, &i);
  }

  EXPECT_EQ(ncsdk_List_Size(list), 10);

  for (int i = 9; i >= 0; --i) {
    ncsdk_List_PopBack(list);
    EXPECT_EQ(ncsdk_List_Size(list), i);
  }

  EXPECT_EQ(ncsdk_List_Size(list), 0);

  ncsdk_List_Delete(list);
  }

TEST(ListTest, TestsSize) {
  ncsdk_List* list = ncsdk_List_New(int);
  EXPECT_NE(list, nullptr);

  EXPECT_EQ(ncsdk_List_Size(list), 0);

  for (int i = 0; i < 10; ++i) {
    ncsdk_List_PushBack(list, &i);
    EXPECT_EQ(ncsdk_List_Size(list), i + 1);
  }

  ncsdk_List_Delete(list);
  }

TEST(ListTest, HandlesMassivePushBack) {
  ncsdk_List* list = ncsdk_List_New(int);
  EXPECT_NE(list, nullptr);

  for (int i = 0; i < 100000; ++i) {
    ncsdk_List_PushBack(list, &i);
  }

  EXPECT_EQ(ncsdk_List_Size(list), 100000);

  for (int i = 0; i < 100000; ++i) {
    int* value = ncsdk_List_At(int, list, i);
    EXPECT_EQ(*value, i);
  }

  ncsdk_List_Delete(list);
  }

TEST(ListTest, HandlesRandomPushAndPopWithoutCheck) {
  ncsdk_List* list = ncsdk_List_New(int);
  EXPECT_NE(list, nullptr);

  size_t size = 0;

  for (int i = 0; i < 100000; ++i) {
    if (rand() % 2 == 0) {
      ncsdk_List_PushBack(list, &i);
      ++size;
      EXPECT_EQ(ncsdk_List_Size(list), size);
    } else {
      ncsdk_List_PopBack(list);
      size = size > 0 ? size - 1 : 0;
      EXPECT_EQ(ncsdk_List_Size(list), size);
    }
  }

  ncsdk_List_Delete(list);
  }
