#pragma once

#ifdef __cplusplus
extern "C" {
#endif

#include <stdarg.h>

struct ncsdk_Logger {
  char* logging_namespace;
};
typedef struct ncsdk_Logger ncsdk_Logger;

ncsdk_Logger* ncsdk_Logger_New(const char* logging_namespace);

void ncsdk_Logger_Delete(ncsdk_Logger* self);

void ncsdk_Logger_Debug(const ncsdk_Logger* self,
                             const char* format, ...);

void ncsdk_Logger_Info(const ncsdk_Logger* self, const char* format,
                       ...);

void ncsdk_Logger_Warn(const ncsdk_Logger* self, const char* format,
                       ...);

void ncsdk_Logger_Error(const ncsdk_Logger* self, const char* format,
                        ...);

#ifdef __cplusplus
}
#endif
