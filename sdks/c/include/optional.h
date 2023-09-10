/// @file optional_value.h
/// @brief Optional value types.

#pragma once

#ifdef __cplusplus
extern "C" {
#endif

#include <stdbool.h>
#include <time.h>

/// @brief Defines an optional value type.
/// @param type The type of the optional value.
#define ncsdk_Optional(type) ncsdk_Optional_##type

#define NCSDK_DEFINE_OPTIONAL(type) \
  struct ncsdk_Optional_##type {    \
    type value;                     \
    bool has_value;                 \
  };                                \
  typedef struct ncsdk_Optional_##type ncsdk_Optional_##type

NCSDK_DEFINE_OPTIONAL(bool);
NCSDK_DEFINE_OPTIONAL(char);
NCSDK_DEFINE_OPTIONAL(int);
NCSDK_DEFINE_OPTIONAL(float);
NCSDK_DEFINE_OPTIONAL(time_t);

#ifdef __cplusplus
}
#endif
