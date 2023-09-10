/// @file stack.h
/// @brief Stack interfaces

#pragma once

#ifdef __cplusplus
extern "C" {
#endif

#include <stddef.h>

/// @struct ncsdk_Stack
/// @brief Stack provides a stack data structure.
struct ncsdk_Stack;
typedef struct ncsdk_Stack ncsdk_Stack;

/// @brief Creates a new stack.
/// @param type The type of the elements in the stack
/// @return The new stack
#define ncsdk_Stack_New(type) ncsdk_Stack_NewWithElementSize(sizeof(type))

ncsdk_Stack* ncsdk_Stack_NewWithElementSize(size_t element_size);

/// @brief Destroys an stack.
/// @param self A pointer to the stack
void ncsdk_Stack_Delete(ncsdk_Stack* self);

/// @brief Removes the top element.
/// @param self The stack
void ncsdk_Stack_Pop(ncsdk_Stack* self);

/// @brief Inserts an element at the top.
/// @param self The stack
/// @param value The value to insert into the stack
void ncsdk_Stack_Push(ncsdk_Stack* self, const void* value);

/// @brief Returns the number of elements.
/// @param self The stack
/// @return The number of elements in the stack
size_t ncsdk_Stack_Size(ncsdk_Stack* self);

/// @brief Accesses the top element.
/// @param type The type of the elements in the stack
/// @param self The stack
/// @return The top element in the stack. If the stack is empty, the value is
/// NULL.
#define ncsdk_Stack_Top(type, self) \
  (type*)ncsdk_Stack_TopWithoutType((ncsdk_Stack*)(self))

void* ncsdk_Stack_TopWithoutType(ncsdk_Stack* self);

#ifdef __cplusplus
}
#endif
