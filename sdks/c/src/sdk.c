#include "internal/sdk.h"

#include <argparse.h>
#include <stdlib.h>
#include <time.h>

#include "internal/logger.h"
#include "internal/client.h"
#include "optional.h"

#define GET_INFO_INVERNAL 10000
#define PING_INTERVAL 1000

static struct ncsdk_Agent *agent_ = NULL;
static struct ncsdk_BlockSource *block_source_ = NULL;
static struct ncsdk_Client *client_ = NULL;
static struct ncsdk_EntitySource *entity_source_ = NULL;
static ncsdk_Optional(int) last_tick_ = {-1, false};
static ncsdk_Optional(time_t) last_tick_time_ = {0, false};
static ncsdk_Optional(float) latency_ = {0.0f, false};
static struct ncsdk_Logger *sdk_logger_ = NULL;
static ncsdk_Optional(float) ticks_per_second_ = {0.0f, false};
static struct ncsdk_Logger *user_logger_ = NULL;
static char *token_ = NULL;

const struct ncsdk_Logger *ncsdk_GetSdkLogger() { return sdk_logger_; }

void ncsdk_Initialize(int argc, char *argv[]) {
  sdk_logger_ = ncsdk_Logger_New("Sdk");
  user_logger_ = ncsdk_Logger_New("User");

  ncsdk_Logger_Info(sdk_logger_, "Initializing SDK...");

  const char *host = "localhost";
  int port = 14514;

  struct argparse_option options[] = {
      OPT_STRING('\0', "token", &token_, "Token to use for authentication",
                 NULL, 0, 0),
      OPT_STRING('\0', "host", &host, "Host to connect to", NULL, 0, 0),
      OPT_INTEGER('\0', "port", &port, "Port to connect to", NULL, 0, 0),
      OPT_END(),
  };

  struct argparse argparse;
  argparse_init(&argparse, options, NULL, 0);
  argparse_parse(&argparse, argc, (const char **)argv);
}

void ncsdk_Finalize() {
  // If the SDK is not initialized, directly exit.
  if (sdk_logger_ == NULL || user_logger_ == NULL) {
    exit(1);
  }

  ncsdk_Logger_Info(sdk_logger_, "Finalizing SDK...");

  ncsdk_Logger_Delete(user_logger_);
  user_logger_ = NULL;

  ncsdk_Logger_Delete(sdk_logger_);
  sdk_logger_ = NULL;
}

struct ncsdk_Agent *ncsdk_GetAgent() { return agent_; }

const struct ncsdk_BlockSource *ncsdk_GetBlocks() { return block_source_; }

struct ncsdk_Client *ncsdk_GetClient() { return client_; }

const struct ncsdk_EntitySource *ncsdk_GetEntities() { return entity_source_; }

ncsdk_Optional(float) ncsdk_GetLatency() { return latency_; }

const struct ncsdk_Logger *ncsdk_GetLogger() { return user_logger_; }

ncsdk_Optional(int) ncsdk_GetTick() {
  if (!last_tick_.has_value || !last_tick_time_.has_value) {
    return (ncsdk_Optional(int)){-1, false};
  }

  if (!ticks_per_second_.has_value) {
    return last_tick_;
  }

  double elapsed = difftime(time(NULL), last_tick_time_.value);
  return (ncsdk_Optional(int)){
      last_tick_.value + (int)(elapsed * ticks_per_second_.value), true};
}

ncsdk_Optional(float) ncsdk_GetTicksPerSecond() { return ticks_per_second_; }

void ncsdk_Refresh() {
  if (client_ == NULL) {
    return;
  }

  ncsdk_Client_Refresh(client_);
}
