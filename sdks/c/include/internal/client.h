#pragma once

#ifdef __cplusplus
extern "C" {
#endif

#include <libwebsockets.h>

#include "internal/logger.h"
#include "internal/message.h"
#include "queue.h"

struct ncsdk_Client {
  float bandwidth;
  ncsdk_Logger* logger;
  void (*message_handler)(const ncsdk_Message*);
  struct lws_context* lws_context;
  struct lws_protocols* lws_protocols;
  struct lws* lws_wsi;
  ncsdk_Queue* message_queue;
};
typedef struct ncsdk_Client ncsdk_Client;

ncsdk_Client* ncsdk_Client_New(const char* host, int port);

void ncsdk_Client_Delete(ncsdk_Client* self);

float ncsdk_Client_GetBandwidth(ncsdk_Client* self);

void ncsdk_Client_Refresh(ncsdk_Client* self);

void ncsdk_Client_RegisterMessageHandler(ncsdk_Client* self,
                                         void (*handler)(const ncsdk_Message*));

void ncsdk_Client_Send(ncsdk_Client* self, const ncsdk_Message* message);

#ifdef __cplusplus
}
#endif
