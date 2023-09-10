#include <array.h>
#include <gtest/gtest.h>

TEST(ArrayTest, TestsValue) {
  ncsdk_Array* array = ncsdk_Array_New(int, 10);
  EXPECT_NE(array, nullptr);

  for (size_t i = 0; i < ncsdk_Array_Size(array); ++i) {
    int* value = ncsdk_Array_At(int, array, i);
    EXPECT_NE(value, nullptr);
    *value = (int)i;
  }

  for (size_t i = 0; i < ncsdk_Array_Size(array); ++i) {
    int* value = ncsdk_Array_At(int, array, i);
    EXPECT_NE(value, nullptr);
    EXPECT_EQ(*value, i);
  }

  ncsdk_Array_Delete(array);
}

TEST(ArrayTest, TestsPointer) {
  ncsdk_Array* array = ncsdk_Array_New(char*, 10);
  EXPECT_NE(array, nullptr);

  for (size_t i = 0; i < ncsdk_Array_Size(array); ++i) {
    char** value = ncsdk_Array_At(char*, array, i);
    EXPECT_NE(value, nullptr);
    *value = new char[10];
    sprintf(*value, "%d", (int)i);
  }

  for (size_t i = 0; i < ncsdk_Array_Size(array); ++i) {
    char** value = ncsdk_Array_At(char*, array, i);
    EXPECT_NE(value, nullptr);
    char expected[10];
    sprintf(expected, "%d", (int)i);
    EXPECT_STREQ(*value, expected);
  }

  for (size_t i = 0; i < ncsdk_Array_Size(array); ++i) {
    char** value = ncsdk_Array_At(char*, array, i);
    EXPECT_NE(value, nullptr);
    delete[] * value;
  }

  ncsdk_Array_Delete(array);
}

TEST(ArrayTest, TestsFilling) {
  ncsdk_Array* array = ncsdk_Array_New(int, 10);
  EXPECT_NE(array, nullptr);

  for (size_t i = 0; i < ncsdk_Array_Size(array); ++i) {
    int* value = ncsdk_Array_At(int, array, i);
    EXPECT_NE(value, nullptr);
    *value = (int)i;
  }

  int v = 0;
  ncsdk_Array_Fill(array, &v);

  for (size_t i = 0; i < ncsdk_Array_Size(array); ++i) {
    int* value = ncsdk_Array_At(int, array, i);
    EXPECT_NE(value, nullptr);
    EXPECT_EQ(*value, 0);
  }

  ncsdk_Array_Delete(array);
}

TEST(ArrayTest, HandlesOutOfRange) {
  ncsdk_Array* array = ncsdk_Array_New(int, 10);
  EXPECT_NE(array, nullptr);

  int* value = ncsdk_Array_At(int, array, 10);

  EXPECT_EQ(value, nullptr);

  value = ncsdk_Array_At(int, array, -1);

  EXPECT_EQ(value, nullptr);

  ncsdk_Array_Delete(array);
}

TEST(ArrayTest, HandlesZeroSize) {
  ncsdk_Array* array = ncsdk_Array_New(int, 0);
  EXPECT_NE(array, nullptr);

  EXPECT_EQ(ncsdk_Array_Size(array), 0);

  int* value = ncsdk_Array_At(int, array, 0);

  EXPECT_EQ(value, nullptr);

  ncsdk_Array_Delete(array);
}
