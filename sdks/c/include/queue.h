/// @file queue.h
/// @brief Queue interfaces

#pragma once

#ifdef __cplusplus
extern "C" {
#endif

#include <stddef.h>

/// @struct ncsdk_Queue
/// @brief Queue provides a queue data structure.
struct ncsdk_Queue;
typedef struct ncsdk_Queue ncsdk_Queue;

/// @brief Creates a new queue.
/// @param type The type of the elements in the queue
/// @return The new queue
#define ncsdk_Queue_New(type) ncsdk_Queue_NewWithElementSize(sizeof(type))

ncsdk_Queue* ncsdk_Queue_NewWithElementSize(size_t element_size);

/// @brief Destroys an queue.
/// @param self A pointer to the queue
void ncsdk_Queue_Delete(ncsdk_Queue* self);

/// @brief Accesses the last element.
/// @param type The type of the elements in the queue
/// @param self The queue
/// @return The last element in the queue. If the queue is empty, the value is
/// NULL.
#define ncsdk_Queue_Back(type, self) \
  (type*)ncsdk_Queue_BackWithoutType((ncsdk_Queue*)(self))

void* ncsdk_Queue_BackWithoutType(ncsdk_Queue* self);

/// @brief Accesses the first element.
/// @param type The type of the elements in the queue
/// @param self The queue
/// @return The first element in the queue. If the queue is empty, the value is
/// NULL.
#define ncsdk_Queue_Front(type, self) \
  (type*)ncsdk_Queue_FrontWithoutType((ncsdk_Queue*)(self))

void* ncsdk_Queue_FrontWithoutType(ncsdk_Queue* self);

/// @brief Removes the first element.
/// @param self The queue
void ncsdk_Queue_Pop(ncsdk_Queue* self);

/// @brief Inserts an element at the end.
/// @param self The queue
/// @param value The value to insert into the queue
void ncsdk_Queue_Push(ncsdk_Queue* self, const void* value);

/// @brief Returns the number of elements.
/// @param self The queue
/// @return The number of elements in the queue
size_t ncsdk_Queue_Size(ncsdk_Queue* self);

#ifdef __cplusplus
}
#endif
