/// @file agent.h
/// @brief NovelCraft Agent interfaces

#pragma once

#ifdef __cplusplus
extern "C" {
#endif

#include "inventory.h"
#include "orientation.h"
#include "position.h"

/// @brief Represents the kind of interaction that an agent can perform.
enum ncsdk_Agent_InteractionKind {
  /// @brief Clicks.
  ncsdk_Agent_InteractionKind_Click,

  /// @brief Starts holding.
  ncsdk_Agent_InteractionKind_HoldStart,

  /// @brief Ends holding.
  ncsdk_Agent_InteractionKind_HoldEnd
};

/// @brief Represents the kind of movement that an agent can perform.
enum ncsdk_Agent_MovementKind {
  /// @brief Moves forward.
  ncsdk_Agent_MovementKind_Forward,

  /// @brief Moves backward.
  ncsdk_Agent_MovementKind_Backward,

  /// @brief Moves left.
  ncsdk_Agent_MovementKind_Left,

  /// @brief Moves right.
  ncsdk_Agent_MovementKind_Right,
};

/// @struct ncsdk_Agent
/// @brief Represents a player that can be controlled by an AI.
struct ncsdk_Agent;
typedef struct ncsdk_Agent ncsdk_Agent;

/// @brief Gets the agent's orientation.
/// @param self The agent.
/// @return The agent's orientation.
const ncsdk_Orientation* ncsdk_Agent_GetOrientation(const ncsdk_Agent* self);

/// @brief Gets the agent's position.
/// @param self The agent.
/// @return The agent's position.
const ncsdk_Position(float) ncsdk_Agent_GetPosition(const ncsdk_Agent* self);

/// @brief Gets the agent's type ID.
/// @param self The agent.
/// @return The agent's type ID.
int ncsdk_Agent_GetTypeId(const ncsdk_Agent* self);

/// @brief Gets the agent's unique ID.
/// @param self The agent.
/// @return The agent's unique ID.
int ncsdk_Agent_GetUniqueId(const ncsdk_Agent* self);

/// @brief Gets the agent's health.
/// @param self The agent.
/// @return The agent's health.
float ncsdk_Agent_GetHealth(const ncsdk_Agent* self);

/// @brief Gets the agent's inventory.
/// @param self The agent.
/// @return The agent's inventory.
const ncsdk_Inventory* ncsdk_Agent_GetInventory(const ncsdk_Agent* self);

/// @brief Gets the agent's movement.
/// @param self The agent.
/// @return The agent's movement.
enum ncsdk_Agent_MovementKind ncsdk_Agent_GetMovement(const ncsdk_Agent* self);

/// @brief Sets the agent's movement.
/// @param self The agent.
/// @param movement The movement.
void ncsdk_Agent_SetMovement(ncsdk_Agent* self,
                             enum ncsdk_Agent_MovementKind movement);

/// @brief Gets the agent's token.
/// @param self The agent.
/// @return The agent's token.
const char* ncsdk_Agent_GetToken(const ncsdk_Agent* self);

/// @brief Performs an attack.
/// @param self The agent.
/// @param kind The kind of attack.
void ncsdk_Agent_Attack(ncsdk_Agent* self,
                        enum ncsdk_Agent_InteractionKind kind);

/// @brief Performs a jump.
/// @param self The agent.
void ncsdk_Agent_Jump(ncsdk_Agent* self);

/// @brief Makes the agent look at a position.
/// @param self The agent.
/// @param position The position.
void ncsdk_Agent_LookAt(ncsdk_Agent* self,
                        const ncsdk_Position(float) position);

/// @brief Sets the agent's movement.
/// @param self The agent.
/// @param movement The movement.
void ncsdk_Agent_SetMovement(ncsdk_Agent* self,
                             enum ncsdk_Agent_MovementKind movement);

/// @brief Performs a use action.
/// @param self The agent.
/// @param kind The kind of use action.
void ncsdk_Agent_Use(ncsdk_Agent* self, enum ncsdk_Agent_InteractionKind kind);

#ifdef __cplusplus
}
#endif
