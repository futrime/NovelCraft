#include <internal/client.h>
#include <sdk.h>
#include <stdio.h>

int main() {
  ncsdk_Client* client = ncsdk_Client_New("localhost", 14514);
  for (int i = 0; i < 10; ++i) {
    ncsdk_Client_Refresh(client);
  }
  ncsdk_Client_Delete(client);
  return 0;
}
