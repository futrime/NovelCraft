#include <gtest/gtest.h>
#include <queue.h>
#include <stdlib.h>

TEST(QueueTest, TestsValue) {
  ncsdk_Queue* queue = ncsdk_Queue_New(int);
  EXPECT_NE(queue, nullptr);

  for (size_t i = 0; i < 10; ++i) {
    int v = (int)i;
    ncsdk_Queue_Push(queue, &v);
    EXPECT_EQ(ncsdk_Queue_Size(queue), i + 1);
  }

  for (size_t i = 0; i < 10; ++i) {
    ncsdk_Queue_Pop(queue);
    EXPECT_EQ(ncsdk_Queue_Size(queue), 9 - i);
  }

  ncsdk_Queue_Delete(queue);
  }

TEST(QueueTest, TestsPointer) {
  ncsdk_Queue* queue = ncsdk_Queue_New(char*);
  EXPECT_NE(queue, nullptr);

  for (size_t i = 0; i < 10; ++i) {
    char* v = new char[10];
    ncsdk_Queue_Push(queue, &v);
    EXPECT_EQ(ncsdk_Queue_Size(queue), i + 1);
  }

  for (size_t i = 0; i < 10; ++i) {
    ncsdk_Queue_Pop(queue);
    EXPECT_EQ(ncsdk_Queue_Size(queue), 9 - i);
  }

  ncsdk_Queue_Delete(queue);
  }

TEST(QueueTest, TestsFrontAndBack) {
  ncsdk_Queue* queue = ncsdk_Queue_New(int);
  EXPECT_NE(queue, nullptr);

  for (size_t i = 0; i < 10; ++i) {
    int v = (int)i;
    ncsdk_Queue_Push(queue, &v);
    EXPECT_EQ(ncsdk_Queue_Size(queue), i + 1);
  }

  for (size_t i = 0; i < 10; ++i) {
    int* v = ncsdk_Queue_Front(int, queue);
    EXPECT_EQ(*v, i);
    v = ncsdk_Queue_Back(int, queue);
    EXPECT_EQ(*v, 9);

    ncsdk_Queue_Pop(queue);
  }

  ncsdk_Queue_Delete(queue);
  }

TEST(QueueTest, HandlesRandomPushAndPopWithoutCheck) {
  ncsdk_Queue* queue = ncsdk_Queue_New(int);
  EXPECT_NE(queue, nullptr);

  size_t size = 0;
  for (size_t i = 0; i < 100000; ++i) {
    int v = (int)i;

    if (rand() % 2 == 0) {
      ncsdk_Queue_Push(queue, &v);
      ++size;
      EXPECT_EQ(ncsdk_Queue_Size(queue), size);
    } else {
      ncsdk_Queue_Pop(queue);
      size = size > 0 ? size - 1 : 0;
      EXPECT_EQ(ncsdk_Queue_Size(queue), size);
    }
  }

  ncsdk_Queue_Delete(queue);
  }
