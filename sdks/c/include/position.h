/// @file position.h
/// @brief NovelCraft Position interfaces

#pragma once

#ifdef __cplusplus
extern "C" {
#endif

#define ncsdk_Position(type) ncsdk_Position_##type

#define NCSDK_DEFINE_POSITION(type) \
  struct ncsdk_Position_##type {    \
    type x, y, z;                   \
  };                                \
  typedef struct ncsdk_Position_##type ncsdk_Position_##type

NCSDK_DEFINE_POSITION(int);
NCSDK_DEFINE_POSITION(float);

#ifdef __cplusplus
}
#endif
