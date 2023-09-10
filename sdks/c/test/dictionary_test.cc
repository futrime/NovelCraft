#include <dictionary.h>
#include <gtest/gtest.h>
#include <stdlib.h>

TEST(DictionaryTest, TestsValueToValue) {
  ncsdk_Dictionary* dict = ncsdk_Dictionary_New(int, int);
  EXPECT_NE(dict, nullptr);

  for (size_t i = 0; i < 10; ++i) {
    int k = (int)i;
    int v = (int)i;
    ncsdk_Dictionary_Insert(dict, &k, &v);
    EXPECT_EQ(ncsdk_Dictionary_Size(dict), i + 1);
  }

  for (size_t i = 0; i < 10; ++i) {
    int k = (int)i;
    int* v = ncsdk_Dictionary_At(int, dict, &k);
    EXPECT_EQ(*v, i);
  }

  ncsdk_Dictionary_Delete(dict);
  }

TEST(DictionaryTest, TestsClear) {
  ncsdk_Dictionary* dict = ncsdk_Dictionary_New(int, int);
  EXPECT_NE(dict, nullptr);

  for (size_t i = 0; i < 10; ++i) {
    int k = (int)i;
    int v = (int)i;
    ncsdk_Dictionary_Insert(dict, &k, &v);
    EXPECT_EQ(ncsdk_Dictionary_Size(dict), i + 1);
  }

  ncsdk_Dictionary_Clear(dict);
  EXPECT_EQ(ncsdk_Dictionary_Size(dict), 0);

  ncsdk_Dictionary_Delete(dict);
  }

TEST(DictionaryTest, TestsContains) {
  ncsdk_Dictionary* dict = ncsdk_Dictionary_New(int, int);
  EXPECT_NE(dict, nullptr);

  for (size_t i = 0; i < 10; ++i) {
    int k = (int)i;
    int v = (int)i;
    ncsdk_Dictionary_Insert(dict, &k, &v);
    EXPECT_EQ(ncsdk_Dictionary_Size(dict), i + 1);
  }

  for (size_t i = 0; i < 10; ++i) {
    int k = (int)i;
    EXPECT_TRUE(ncsdk_Dictionary_Contains(dict, &k));
  }

  for (size_t i = 10; i < 20; ++i) {
    int k = (int)i;
    EXPECT_FALSE(ncsdk_Dictionary_Contains(dict, &k));
  }

  ncsdk_Dictionary_Delete(dict);
  }

TEST(DictionaryTest, TestsErase) {
  ncsdk_Dictionary* dict = ncsdk_Dictionary_New(int, int);
  EXPECT_NE(dict, nullptr);

  for (size_t i = 0; i < 10; ++i) {
    int k = (int)i;
    int v = (int)i;
    ncsdk_Dictionary_Insert(dict, &k, &v);
    EXPECT_EQ(ncsdk_Dictionary_Size(dict), i + 1);
  }

  for (size_t i = 0; i < 10; ++i) {
    int k = (int)i;
    ncsdk_Dictionary_Erase(dict, &k);
    EXPECT_EQ(ncsdk_Dictionary_Size(dict), 9 - i);
  }

  ncsdk_Dictionary_Delete(dict);
  }

TEST(DictionaryTest, HandlesErasureNonExistent) {
  ncsdk_Dictionary* dict = ncsdk_Dictionary_New(int, int);
  EXPECT_NE(dict, nullptr);

  for (size_t i = 0; i < 10; ++i) {
    int k = (int)i;
    int v = (int)i;
    ncsdk_Dictionary_Insert(dict, &k, &v);
    EXPECT_EQ(ncsdk_Dictionary_Size(dict), i + 1);
  }

  for (size_t i = 10; i < 20; ++i) {
    int k = (int)i;
    ncsdk_Dictionary_Erase(dict, &k);
    EXPECT_EQ(ncsdk_Dictionary_Size(dict), 10);
  }

  ncsdk_Dictionary_Delete(dict);
  }

TEST(DictionaryTest, HandlesErasureAfterClear) {
  ncsdk_Dictionary* dict = ncsdk_Dictionary_New(int, int);
  EXPECT_NE(dict, nullptr);

  for (size_t i = 0; i < 10; ++i) {
    int k = (int)i;
    int v = (int)i;
    ncsdk_Dictionary_Insert(dict, &k, &v);
    EXPECT_EQ(ncsdk_Dictionary_Size(dict), i + 1);
  }

  ncsdk_Dictionary_Clear(dict);
  EXPECT_EQ(ncsdk_Dictionary_Size(dict), 0);

  for (size_t i = 0; i < 10; ++i) {
    int k = (int)i;
    ncsdk_Dictionary_Erase(dict, &k);
    EXPECT_EQ(ncsdk_Dictionary_Size(dict), 0);
  }

  ncsdk_Dictionary_Delete(dict);
  }

TEST(DictionaryTest, HandlesKeyNotFound) {
  ncsdk_Dictionary* dict = ncsdk_Dictionary_New(int, int);
  EXPECT_NE(dict, nullptr);

  for (size_t i = 0; i < 10; ++i) {
    int k = (int)i;
    int v = (int)i;
    ncsdk_Dictionary_Insert(dict, &k, &v);
    EXPECT_EQ(ncsdk_Dictionary_Size(dict), i + 1);
  }

  for (size_t i = 10; i < 20; ++i) {
    int k = (int)i;
    int* v = ncsdk_Dictionary_At(int, dict, &k);
    EXPECT_EQ(v, nullptr);
  }

  ncsdk_Dictionary_Delete(dict);
  }

TEST(DictionaryTest, HandlesDuplicatedInsertion) {
  ncsdk_Dictionary* dict = ncsdk_Dictionary_New(int, int);
  EXPECT_NE(dict, nullptr);

  for (size_t i = 0; i < 10; ++i) {
    int k = (int)i;
    int v = (int)i;
    ncsdk_Dictionary_Insert(dict, &k, &v);
    EXPECT_EQ(ncsdk_Dictionary_Size(dict), i + 1);
  }

  for (size_t i = 0; i < 10; ++i) {
    int k = (int)i;
    int v = (int)i + 1;
    ncsdk_Dictionary_Insert(dict, &k, &v);
    EXPECT_EQ(ncsdk_Dictionary_Size(dict), 10);
  }

  for (size_t i = 0; i < 10; ++i) {
    int k = (int)i;
    int* v = ncsdk_Dictionary_At(int, dict, &k);
    EXPECT_EQ(*v, i + 1);
  }

  ncsdk_Dictionary_Delete(dict);
  }
