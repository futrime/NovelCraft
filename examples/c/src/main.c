#include <sdk.h>
#include <time.h>

int main(int argc, char* argv[]) {
  // Initialize the SDK
  ncsdk_Initialize(argc, argv);
  ncsdk_Logger_Info(ncsdk_GetLogger(), "Hello, world!");

  while (true) {
    ncsdk_Refresh();

    ncsdk_Agent* agent = ncsdk_GetAgent();

    if (agent == NULL) {
      continue;
    }

    ncsdk_Agent_SetMovement(agent, ncsdk_Agent_MovementKind_Forward);
  }

  // Finalize the SDK
  ncsdk_Finalize();
  ncsdk_Logger_Info(ncsdk_GetLogger(), "Goodbye, world!");

  return 0;
}
