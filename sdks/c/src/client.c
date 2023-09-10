#include "internal/client.h"

#include <jansson.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "internal/inventory.h"
#include "internal/logger.h"
#include "internal/message.h"
#include "internal/sdk.h"
#include "list.h"
#include "queue.h"

#define RECV_BUF_SIZE 114514
#define MAX_MESSAGE_LENGTH 128

static int Callback(struct lws* wsi, enum lws_callback_reasons reason,
                    void* user, void* in, size_t len);

struct ncsdk_Client* ncsdk_Client_New(const char* host, int port) {
  // Initialize the queue

  struct ncsdk_Client* client = malloc(sizeof(struct ncsdk_Client));

  client->message_queue = ncsdk_Queue_New(char*);

  client->bandwidth = 0.0f;

  client->logger = ncsdk_Logger_New("Sdk.Client");

  client->lws_protocols = malloc(2 * sizeof(struct lws_protocols));
  memset(client->lws_protocols, 0, 2 * sizeof(struct lws_protocols));
  client->lws_protocols[0].name = "default";
  client->lws_protocols[0].callback = Callback;
  client->lws_protocols[0].per_session_data_size = 0;
  client->lws_protocols[0].rx_buffer_size = RECV_BUF_SIZE;
  client->lws_protocols[0].user = client;

  struct lws_context_creation_info info;
  memset(&info, 0, sizeof(info));
  info.port = CONTEXT_PORT_NO_LISTEN;
  info.protocols = client->lws_protocols;
  info.gid = -1;
  info.uid = -1;

  client->lws_context = lws_create_context(&info);

  struct lws_client_connect_info ccinfo;
  memset(&ccinfo, 0, sizeof(ccinfo));
  ccinfo.context = client->lws_context;
  ccinfo.address = host;
  ccinfo.port = port;
  ccinfo.path = "/";
  ccinfo.host = lws_canonical_hostname(client->lws_context);
  ccinfo.origin = "origin";
  ccinfo.protocol = client->lws_protocols[0].name;

  client->lws_wsi = lws_client_connect_via_info(&ccinfo);

  return client;
}

void ncsdk_Client_Delete(struct ncsdk_Client* self) {
  ncsdk_Logger_Delete(self->logger);
  lws_context_destroy(self->lws_context);
  free(self->lws_protocols);
  ncsdk_Queue_Delete(self->message_queue);

  free(self);
}

float ncsdk_Client_GetBandwidth(ncsdk_Client* self) { return self->bandwidth; }

void ncsdk_Client_Refresh(ncsdk_Client* self) {
  lws_service(self->lws_context, 0);
}

void ncsdk_Client_RegisterMessageHandler(
    ncsdk_Client* self, void (*handler)(const ncsdk_Message*)) {
  self->message_handler = handler;
}

void ncsdk_Client_Send(ncsdk_Client* self, const ncsdk_Message* message) {
  char* json_string = ncsdk_Message_GetJsonString(message);
  if (ncsdk_Queue_Size(self->message_queue) < MAX_MESSAGE_LENGTH) {
    ncsdk_Queue_Push(self->message_queue, json_string);
  } else {
    // Too many messages
  }
  free(json_string);
}

static int Callback(struct lws* wsi, enum lws_callback_reasons reason,
                    void* user, void* in, size_t len) {
  if (user == NULL) {
    return 0;
  }

  ncsdk_Client* client = (ncsdk_Client*)user;
  switch (reason) {
    case LWS_CALLBACK_CLIENT_ESTABLISHED: {
      ncsdk_Logger_Debug(client->logger, "Connection established.");
      break;
    }

    case LWS_CALLBACK_CLIENT_RECEIVE: {
      ncsdk_Logger_Debug(client->logger, "Received message.");
      char* json_string = (char*)in;
      ncsdk_Message* message = ncsdk_Message_New(json_string);
      enum ncsdk_Message_MessageKind message_kind =
          ncsdk_Message_GetType(message);

      if (ncsdk_Message_GetBoundTo(message) !=
          ncsdk_Message_BoundToKind_ClientBound)
        return 0;

      switch (message_kind) {
        case ncsdk_Message_MessageKind_AfterBlockChange: {
          ncsdk_BlockSource* block_source = ncsdk_GetBlocks();
          // Traverse
          json_t* change_list = json_object_get(message, "change_list");
          int change_list_size = json_array_size(change_list);
          for (int i = 0; i < change_list_size; i++) {
            json_t* change_info = json_array_get(change_list, i);
            json_t* position_json = json_object_get(change_info, "position");
            if (position_json == NULL) {
              ncsdk_Logger_Error(client->logger,
                                 "position in the message "
                                 "<AfterBlockChange> doesn't exist!");
              json_decref(change_info);
              return 0;
            }
            ncsdk_Position_int* position = malloc(sizeof(ncsdk_Position_int));

            json_t* x_json = json_object_get(position_json, "x");
            if (position_json == NULL) {
              ncsdk_Logger_Error(client->logger,
                                 "position.x in the message "
                                 "<AfterBlockChange> is invalid!");
              json_decref(change_info);
              free(position);
              return 0;
            }
            position->x = json_integer_value(x_json);
            json_decref(x_json);

            json_t* y_json = json_object_get(position_json, "y");
            if (position_json == NULL) {
              ncsdk_Logger_Error(client->logger,
                                 "position.y in the message "
                                 "<AfterBlockChange> is invalid!");
              json_decref(change_info);
              free(position);
              return 0;
            }
            position->y = json_integer_value(y_json);
            json_decref(y_json);

            json_t* z_json = json_object_get(position_json, "z");
            if (position_json == NULL) {
              ncsdk_Logger_Error(client->logger,
                                 "position.z in the message "
                                 "<AfterBlockChange> is invalid!");
              json_decref(change_info);
              free(position);
              return 0;
            }
            position->z = json_integer_value(z_json);

            json_t* block_type_id_json =
                json_object_get(change_info, "block_type_id");
            if (block_type_id_json == NULL) {
              ncsdk_Logger_Error(client->logger,
                                 "block_type_id in the message "
                                 "<AfterBlockChange> is invalid!");
              json_decref(change_info);
              free(position);
              return 0;
            }
            int block_type_id = json_integer_value(block_type_id_json);

            ncsdk_Block* block =
                ncsdk_BlockSource_GetBlock(block_source, position);
            if (block != NULL) {
              block->type_id = block_type_id;
              free(block);
            }
            json_decref(change_info);
            json_decref(position_json);
          }
        } break;

        case ncsdk_Message_MessageKind_AfterEntityAttack: {
          // No implementation
        } break;
        case ncsdk_Message_MessageKind_AfterEntityCreate: {
          ncsdk_EntitySource* entity_source = ncsdk_GetEntities();
          json_t* creation_list = json_object_get(message, "creation_list");
          if (creation_list == NULL) {
            ncsdk_Logger_Error(client->logger,
                               "No change_list in the message "
                               "<AfterEntityCreate>!");
            return 0;
          }
          int creation_list_size = json_array_size(creation_list);

          for (int i = 0; i < creation_list_size; i++) {
            json_t* creation_info = json_array_get(creation_list, i);

            json_t* entity_type_id_json =
                json_object_get(creation_info, "entity_type_id");
            int entity_type_id = json_integer_value(entity_type_id_json);

            json_t* unique_id_json =
                json_object_get(creation_info, "unique_id");
            int unique_id = json_integer_value(unique_id_json);

            // position
            json_t* position_json = json_object_get(creation_info, "position");

            ncsdk_Position_float position;
            json_t* x_json = json_object_get(position_json, "x");
            if (position_json == NULL) {
              ncsdk_Logger_Error(client->logger,
                                 "position.x of entity(id:%d) in the message "
                                 "<AfterEntityCreate> is invalid!",
                                 unique_id);
              json_decref(creation_info);
              return 0;
            }
            position.x = json_real_value(x_json);
            json_decref(x_json);

            json_t* y_json = json_object_get(position_json, "y");
            if (position_json == NULL) {
              ncsdk_Logger_Error(client->logger,
                                 "position.y of entity(id:%d) in the message "
                                 "<AfterEntityCreate> is invalid!",
                                 unique_id);
              json_decref(creation_info);
              return 0;
            }
            position.y = json_real_value(y_json);
            json_decref(y_json);

            json_t* z_json = json_object_get(position_json, "z");
            if (position_json == NULL) {
              ncsdk_Logger_Error(client->logger,
                                 "position.z of entity(id:%d) in the message "
                                 "<AfterEntityCreate> is invalid!",
                                 unique_id);
              json_decref(creation_info);
              return 0;
            }
            position.z = json_real_value(z_json);

            json_decref(position_json);

            // orientation
            json_t* orientation_json =
                json_object_get(creation_info, "orientation");

            ncsdk_Orientation orientation;
            json_t* pitch_json = json_object_get(orientation_json, "pitch");
            if (pitch_json == NULL) {
              ncsdk_Logger_Error(client->logger,
                                 "pitch in the message "
                                 "<AfterEntityCreate> is invalid!");
              json_decref(creation_info);
              return 0;
            }
            orientation.pitch = json_real_value(pitch_json);

            json_t* yaw_json = json_object_get(orientation_json, "yaw");
            if (yaw_json == NULL) {
              ncsdk_Logger_Error(client->logger,
                                 "yaw in the message "
                                 "<AfterEntityCreate> is invalid!");
              json_decref(creation_info);
              return 0;
            }
            orientation.yaw = json_real_value(yaw_json);

            json_decref(orientation_json);

            // item_type_id ?? health ??
            ncsdk_EntitySource_AddEntity(
                entity_source, ncsdk_Entity_New(orientation, position,
                                                entity_type_id, unique_id));

            json_decref(creation_info);
          }
          json_decref(creation_list);
        } break;
        case ncsdk_Message_MessageKind_AfterEntityDespawn: {
          // No implementation
        } break;
        case ncsdk_Message_MessageKind_AfterEntityHurt: {
          // No implementation
        } break;
        case ncsdk_Message_MessageKind_AfterEntityOrientationChange: {
          ncsdk_EntitySource* entity_source = ncsdk_GetEntities();
          json_t* change_list = json_object_get(message, "change_list");
          if (change_list == NULL) {
            ncsdk_Logger_Error(client->logger,
                               "No change_list in the message "
                               "<AfterEntityOrientationChange>!");
            return 0;
          }
          int change_list_size = json_array_size(change_list);

          for (int i = 0; i < change_list_size; i++) {
            json_t* change_info = json_array_get(change_list, i);

            json_t* unique_id_json = json_object_get(change_info, "unique_id");
            if (unique_id_json == NULL) {
              ncsdk_Logger_Error(client->logger,
                                 "unique_id in the message "
                                 "<AfterEntityOrientationChange> is invalid!");
              json_decref(change_info);
              return 0;
            }
            int unique_id = json_integer_value(unique_id_json);
            json_decref(change_info);

            // Try to find the entity
            ncsdk_Entity* entity =
                ncsdk_EntitySource_GetEntity(entity_source, unique_id);
            if (entity == NULL) {
              ncsdk_Logger_Error(client->logger,
                                 "unique_id in the message "
                                 "<AfterEntityOrientationChange> is not found "
                                 "in the entity_source!");
              json_decref(change_info);
              return 0;
            }

            // orientation
            json_t* orientation_json =
                json_object_get(change_info, "orientation");

            ncsdk_Orientation orientation;

            json_t* pitch_json = json_object_get(orientation_json, "pitch");
            if (pitch_json == NULL) {
              ncsdk_Logger_Error(client->logger,
                                 "pitch in the message "
                                 "<AfterEntityOrientationChange> is invalid!");
              json_decref(change_info);
              return 0;
            }
            orientation.pitch = json_real_value(pitch_json);

            json_t* yaw_json = json_object_get(orientation_json, "yaw");
            if (yaw_json == NULL) {
              ncsdk_Logger_Error(client->logger,
                                 "yaw in the message "
                                 "<AfterEntityOrientationChange> is invalid!");
              json_decref(change_info);
              return 0;
            }
            orientation.yaw = json_real_value(yaw_json);

            json_decref(orientation_json);

            // Update the orientation
            entity->orientation = orientation;

            json_decref(change_info);
          }
          json_decref(change_list);

        } break;
        case ncsdk_Message_MessageKind_AfterEntityPositionChange: {
          ncsdk_EntitySource* entity_source = ncsdk_GetEntities();
          json_t* change_list = json_object_get(message, "change_list");
          int change_list_size = json_array_size(change_list);

          for (int i = 0; i < change_list_size; i++) {
            json_t* change_info = json_array_get(change_list, i);

            json_t* unique_id_json = json_object_get(change_info, "unique_id");
            if (unique_id_json == NULL) {
              ncsdk_Logger_Error(client->logger,
                                 "unique_id in the message "
                                 "<AfterEntityPositionChange> is invalid!");
              json_decref(change_info);
              return 0;
            }
            int unique_id = json_integer_value(unique_id_json);
            json_decref(change_info);

            // Try to find the entity
            ncsdk_Entity* entity =
                ncsdk_EntitySource_GetEntity(entity_source, unique_id);
            if (entity == NULL) {
              ncsdk_Logger_Error(client->logger,
                                 "unique_id in the message "
                                 "<AfterEntityPositionChange> is not found "
                                 "in the entity_source!");
              json_decref(change_info);
              return 0;
            }

            // position
            json_t* position_json = json_object_get(change_list, "position");
            ncsdk_Position_float position;
            json_t* x_json = json_object_get(position_json, "x");
            if (position_json == NULL) {
              ncsdk_Logger_Error(client->logger,
                                 "position.x of entity(id:%d) in the message "
                                 "<AfterEntityPositionChange> is invalid!",
                                 unique_id);
              json_decref(change_list);
              return 0;
            }
            position.x = json_real_value(x_json);
            json_decref(x_json);

            json_t* y_json = json_object_get(position_json, "y");
            if (position_json == NULL) {
              ncsdk_Logger_Error(client->logger,
                                 "position.y of entity(id:%d) in the message "
                                 "<AfterEntityPositionChange> is invalid!",
                                 unique_id);
              json_decref(change_list);
              return 0;
            }
            position.y = json_real_value(y_json);
            json_decref(y_json);

            json_t* z_json = json_object_get(position_json, "z");
            if (position_json == NULL) {
              ncsdk_Logger_Error(client->logger,
                                 "position.z of entity(id:%d) in the message "
                                 "<AfterEntityPositionChange> is invalid!",
                                 unique_id);
              json_decref(change_list);
              return 0;
            }
            position.z = json_real_value(z_json);

            entity->position = position;

            json_decref(position_json);
          }
        } break;
        case ncsdk_Message_MessageKind_AfterEntityRemove: {
        } break;
        case ncsdk_Message_MessageKind_AfterEntitySpawn: {
        } break;
        case ncsdk_Message_MessageKind_AfterPlayerInventoryChange: {
        } break;
        case ncsdk_Message_MessageKind_GetBlocksAndEntities: {
          // Remove section
          ncsdk_EntitySource* entity_source = ncsdk_GetEntities();
          json_t* change_list = json_object_get(message, "change_list");
          int change_list_size = json_array_size(change_list);
          // Clear the entity source if the message is valid
          ncsdk_EntitySource_Clear(entity_source);
        } break;
        case ncsdk_Message_MessageKind_GetPlayerInfo: {
          ncsdk_Agent* agent = ncsdk_GetAgent();
          if (agent == NULL) {
            ncsdk_Logger_Error(client->logger, "Agent is NULL!");
            return 0;
          }

          json_t* health_json = json_object_get(message, "health");
          if (health_json == NULL) {
            ncsdk_Logger_Error(client->logger,
                               "health of agent in the message "
                               "<GetPlayerInfo> is invalid!");
            return 0;
          }
          agent->health = json_integer_value(health_json);

          json_t* main_hand_json = json_object_get(message, "main_hand");
          if (main_hand_json == NULL) {
            ncsdk_Logger_Error(client->logger,
                               "main_hand_slot of agent in the message "
                               "<GetPlayerInfo> is invalid!");
            return 0;
          }
          ncsdk_Inventory_SetMainHandSlot(agent->inventory,
                                          json_integer_value(main_hand_json));
        } break;
        case ncsdk_Message_MessageKind_GetTick: {
        } break;
        case ncsdk_Message_MessageKind_Error: {
        } break;
      }
      free(message);
    }

    case LWS_CALLBACK_CLIENT_WRITEABLE: {
      ncsdk_Logger_Debug(client->logger, "Writeable.");
      if (ncsdk_Queue_Size(client->message_queue) > 0) {
        char* message_string = ncsdk_Queue_Front(char*, client->message_queue);
        ncsdk_Queue_Pop(client->message_queue);
        int len = lws_write(wsi, message_string, sizeof(message_string),
                            LWS_WRITE_TEXT);
        ncsdk_Logger_Debug(client->logger, "Write %d", len);
      }
      break;
    }

    case LWS_CALLBACK_CLOSED: {
      ncsdk_Logger_Debug(client->logger, "Connection closed.");
      break;
    }

    default: {
      break;
    }
  }

  return 0;
}
