#include "internal/message.h"

#include <jansson.h>
#include <stdio.h>

struct ncsdk_Message* ncsdk_Message_New(const char* json_string) {
  struct json_t* json = json_loads(json_string, 0, NULL);

  struct ncsdk_Message* self = malloc(sizeof(struct ncsdk_Message));
  self->json = json;
  return self;
}

struct ncsdk_Message* ncsdk_Message_NewFromJson(json_t* json) {
  struct ncsdk_Message* message = malloc(sizeof(struct ncsdk_Message));
  message->json = json;
  return message;
}

void ncsdk_Message_Delete(struct ncsdk_Message* self) {
  json_decref(self->json);

  free(self->json);
  free(self);
}

const struct json_t* ncsdk_Message_GetJson(const struct ncsdk_Message* self) {
  return self->json;
}

char* ncsdk_Message_GetJsonString(const struct ncsdk_Message* self) {
  return json_dumps(self->json, 0);
}

enum ncsdk_Message_BoundToKind ncsdk_Message_GetBoundTo(
    const struct ncsdk_Message* self) {
  json_t* bound_to = json_object_get(self->json, "bound_to");
  if (json_is_object(bound_to)) {
    return json_integer_value(bound_to);
  }
  return ncsdk_Message_BoundToKind_Unknown;
}

enum ncsdk_Message_MessageKind ncsdk_Message_GetType(
    const struct ncsdk_Message* self) {
  json_t* type = json_object_get(self->json, "type");
  if (json_is_object(type)) {
    return json_integer_value(type);
  }
  return ncsdk_Message_MessageKind_Unknown;
}
