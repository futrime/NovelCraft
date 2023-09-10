/// @file orientation.h
/// @brief NovelCraft Orientation interfaces

#pragma once

#ifdef __cplusplus
extern "C" {
#endif

struct ncsdk_Orientation {
  float yaw, pitch;
};
typedef struct ncsdk_Orientation ncsdk_Orientation;

#ifdef __cplusplus
}
#endif
