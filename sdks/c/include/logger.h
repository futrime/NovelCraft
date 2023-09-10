/// @file logger.h
/// @brief NovelCraft Logger interfaces

#pragma once

#ifdef __cplusplus
extern "C" {
#endif

#include <stdarg.h>

struct ncsdk_Logger;
typedef struct ncsdk_Logger ncsdk_Logger;

/// @brief Logs a debug message.
/// @param self The logger instance.
/// @param format The message format.
/// @param ... The format arguments.
void ncsdk_Logger_Debug(const ncsdk_Logger* self, const char* format,
                        ...);

/// @brief Logs an info message.
/// @param self The logger instance.
/// @param format The message format.
/// @param ... The format arguments.
void ncsdk_Logger_Info(const ncsdk_Logger* self, const char* format,
                       ...);

/// @brief Logs a warning message.
/// @param self The logger instance.
/// @param format The message format.
/// @param ... The format arguments.
void ncsdk_Logger_Warn(const ncsdk_Logger* self, const char* format,
                       ...);

/// @brief Logs an error message.
/// @param self The logger instance.
/// @param format The message format.
/// @param ... The format arguments.
void ncsdk_Logger_Error(const ncsdk_Logger* self, const char* format,
                        ...);

#ifdef __cplusplus
}
#endif
