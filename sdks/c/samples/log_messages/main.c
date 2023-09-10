#include "sdk.h"

int main(int argc, char *argv[]) {
  ncsdk_Initialize(argc, argv);

  const struct ncsdk_Logger *logger = ncsdk_GetLogger();

  ncsdk_Logger_Debug(logger, "Debug message");
  ncsdk_Logger_Info(logger, "Info message");
  ncsdk_Logger_Warn(logger, "Warn message");
  ncsdk_Logger_Error(logger, "Error message");

  ncsdk_Finalize();

  return 0;
}