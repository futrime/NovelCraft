/// @file dictionary.h
/// @brief Dictionary interfaces

#pragma once

#ifdef __cplusplus
extern "C" {
#endif

#include <stdbool.h>
#include <stddef.h>

#include "list.h"

/// @struct ncsdk_Dictionary
/// @brief Dictionary provides a generic dictionary implementation.
struct ncsdk_Dictionary;
typedef struct ncsdk_Dictionary ncsdk_Dictionary;

/// @brief Creates a new dictionary.
/// @param key_type The type of the keys in the dictionary
/// @param value_type The type of the values in the dictionary
/// @return The new dictionary
#define ncsdk_Dictionary_New(key_type, value_type) \
  ncsdk_Dictionary_NewWithElementSizes(sizeof(key_type), sizeof(value_type))

ncsdk_Dictionary* ncsdk_Dictionary_NewWithElementSizes(size_t key_size,
                                                       size_t value_size);

/// @brief Destroys an dictionary.
/// @param self A pointer to the dictionary
void ncsdk_Dictionary_Delete(ncsdk_Dictionary* self);

/// @brief Accesses the value associated with a key.
/// @param value_type The type of the values in the dictionary
/// @param self The dictionary
/// @param key The key to search for
/// @return The value associated with the key. If the key is not found, the
/// value is NULL.
#define ncsdk_Dictionary_At(value_type, self, key) \
  (value_type*)ncsdk_Dictionary_AtWithoutType((ncsdk_Dictionary*)(self), key)

void* ncsdk_Dictionary_AtWithoutType(ncsdk_Dictionary* self, const void* key);

/// @brief Removes all elements.
/// @param self The dictionary
void ncsdk_Dictionary_Clear(ncsdk_Dictionary* self);

/// @brief Checks if the dictionary contains a key.
/// @param self The dictionary
/// @param key The key to search for
/// @return True if the key is found, false otherwise
bool ncsdk_Dictionary_Contains(ncsdk_Dictionary* self, const void* key);

/// @brief Removes an element.
/// @param self The dictionary
/// @param key The key to search for
void ncsdk_Dictionary_Erase(ncsdk_Dictionary* self, const void* key);

/// @brief Inserts an element.
/// @param self The dictionary
/// @param key The key to insert into the dictionary. If the key already exists,
/// the value is updated.
/// @param value The value to insert into the dictionary
void ncsdk_Dictionary_Insert(ncsdk_Dictionary* self, const void* key,
                             const void* value);

/// @brief Gets the keys in the dictionary.
/// @param self The dictionary
/// @return The keys in the dictionary
const ncsdk_List* ncsdk_Dictionary_GetKeys(ncsdk_Dictionary* self);

/// @brief Gets the values in the dictionary.
/// @param self The dictionary
/// @return The values in the dictionary
const ncsdk_List* ncsdk_Dictionary_GetValues(ncsdk_Dictionary* self);

/// @brief Returns the number of elements.
/// @param self The dictionary
/// @return The number of elements in the dictionary
size_t ncsdk_Dictionary_Size(ncsdk_Dictionary* self);

#ifdef __cplusplus
}
#endif
