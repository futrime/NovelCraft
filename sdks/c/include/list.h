/// @file list.h
/// @brief List interfaces

#pragma once

#ifdef __cplusplus
extern "C" {
#endif

#include <stddef.h>

/// @struct ncsdk_List
/// @brief List provides a dynamic array.
struct ncsdk_List;
typedef struct ncsdk_List ncsdk_List;

/// @brief Creates a new list.
/// @param type The type of the elements in the list
/// @return The new list
#define ncsdk_List_New(type) ncsdk_List_NewWithElementSize(sizeof(type))

/// @brief Creates a new list with specified element size.
/// @param element_size The size of each element in the list
/// @return The new list
ncsdk_List* ncsdk_List_NewWithElementSize(size_t element_size);

/// @brief Destroys an list.
/// @param self A pointer to the list
void ncsdk_List_Delete(ncsdk_List* self);

/// @brief Accesses specified element with bounds checking.
/// @param type The type of the elements in the list
/// @param self The list
/// @param index The index of the element to access
/// @return The element at specified position. If index is out of range, the
/// value is NULL.
#define ncsdk_List_At(type, self, index) \
  (type*)ncsdk_List_AtWithoutType((ncsdk_List*)(self), (size_t)(index))

void* ncsdk_List_AtWithoutType(ncsdk_List* self, size_t index);

/// @brief Clears the list.
/// @param self The list
void ncsdk_List_Clear(ncsdk_List* self);

/// @brief Fills the list with the specified value.
/// @param self The list
/// @param value The value to fill the list with
void ncsdk_List_Fill(ncsdk_List* self, void* value);

/// @brief Erases the element at the specified position.
/// @param self The list
/// @param index The index of the element to erase
void ncsdk_List_Erase(ncsdk_List* self, size_t index);

/// @brief Inserts an element into the list before the specified position.
/// @param self The list
/// @param index The index of the element to insert before
/// @param value The value to insert into the list
void ncsdk_List_Insert(ncsdk_List* self, size_t index,
                                  const void* element);

/// @brief Removes the last element of the list.
/// @param self The list
void ncsdk_List_PopBack(ncsdk_List* self);

/// @brief Adds an element to the end of the list.
/// @param self The list
/// @param value The value to add to the list
void ncsdk_List_PushBack(ncsdk_List* self, const void* element);

/// @brief Returns the number of elements in the list.
/// @param self The list
/// @return The number of elements in the list
size_t ncsdk_List_Size(ncsdk_List* self);

#ifdef __cplusplus
}
#endif
