/// @file array.h
/// @brief Array interfaces

#pragma once

#ifdef __cplusplus
extern "C" {
#endif

#include <stddef.h>

/// @struct ncsdk_Array
/// @brief Represents an array.
struct ncsdk_Array;
typedef struct ncsdk_Array ncsdk_Array;

/// @brief Creates a new array.
/// @param type The type of the element
/// @param size The initial size of the array
/// @return The new array
#define ncsdk_Array_New(type, size) \
  ncsdk_Array_NewWithElementSize(sizeof(type), size)

ncsdk_Array* ncsdk_Array_NewWithElementSize(size_t element_size,
                                            size_t size);

/// @brief Destroys an array.
/// @param self A pointer to the array
void ncsdk_Array_Delete(ncsdk_Array* self);

/// @brief Accesses specified element with bounds checking.
/// @param type The type of the element
/// @param self The array
/// @param index The index of the element to access
/// @return The element at specified position. If index is out of range, the
/// value is NULL.
#define ncsdk_Array_At(type, self, index) \
  (type*)ncsdk_Array_AtWithoutType((ncsdk_Array*)self, (size_t)index)

void* ncsdk_Array_AtWithoutType(ncsdk_Array* self, size_t index);

/// @brief Fills the array with the specified value.
/// @param self The array
/// @param value The value to fill the array with
void ncsdk_Array_Fill(ncsdk_Array* self, void* value);

/// @brief Returns the number of elements in the array.
/// @param self The array
/// @return The number of elements in the array
size_t ncsdk_Array_Size(ncsdk_Array* self);

#ifdef __cplusplus
}
#endif
