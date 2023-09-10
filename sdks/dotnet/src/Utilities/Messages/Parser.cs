using System.Text.Json;
using System.Text.Json.Nodes;

namespace NovelCraft.Utilities.Messages;


/// <summary>
/// Parses messages from JSON.
/// </summary>
internal static class Parser {
  /// <summary>
  /// Parses a message from JSON.
  /// </summary>
  public static IMessage Parse(JsonNode json) {
    return Parse(json.ToJsonString()!);
  }

  /// <summary>
  /// Parses a message from JSON.
  /// </summary>
  public static IMessage Parse(string jsonString) {
    var result = JsonSerializer.Deserialize<EmptyMessage>(jsonString)!;

    IMessage.BoundToKind boundTo = (IMessage.BoundToKind)(int)result.BoundTo!;
    IMessage.MessageKind kind = (IMessage.MessageKind)(int)result.Type!;

    return (boundTo, kind) switch {
      (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.Ping) =>
        JsonSerializer.Deserialize<ClientPingMessage>(jsonString)!,
      (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.Ping) =>
        JsonSerializer.Deserialize<ServerPongMessage>(jsonString)!,


      (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.Error) =>
        JsonSerializer.Deserialize<ErrorMessage>(jsonString)!,


      (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.AfterBlockChange) =>
        JsonSerializer.Deserialize<ServerAfterBlockChangeMessage>(jsonString)!,

      (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.AfterEntityAttack) =>
        JsonSerializer.Deserialize<ServerAfterEntityAttackMessage>(jsonString)!,

      (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.AfterEntityCreate) =>
        JsonSerializer.Deserialize<ServerAfterEntityCreateMessage>(jsonString)!,

      (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.AfterEntityDespawn) =>
        JsonSerializer.Deserialize<ServerAfterEntityDespawnMessage>(jsonString)!,

      (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.AfterEntityHurt) =>
        JsonSerializer.Deserialize<ServerAfterEntityHurtMessage>(jsonString)!,

      (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.AfterEntityOrientationChange) =>
        JsonSerializer.Deserialize<ServerAfterEntityOrientationChangeMessage>(jsonString)!,

      (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.AfterEntityPositionChange) =>
        JsonSerializer.Deserialize<ServerAfterEntityPositionChangeMessage>(jsonString)!,

      (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.AfterEntityRemove) =>
        JsonSerializer.Deserialize<ServerAfterEntityRemoveMessage>(jsonString)!,

      (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.AfterEntitySpawn) =>
        JsonSerializer.Deserialize<ServerAfterEntitySpawnMessage>(jsonString)!,

      (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.AfterPlayerInventoryChange) =>
        JsonSerializer.Deserialize<ServerAfterPlayerInventoryChangeMessage>(jsonString)!,


      (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.GetBlocksAndEntities) =>
        JsonSerializer.Deserialize<ClientGetBlocksAndEntitiesMessage>(jsonString)!,
      (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.GetBlocksAndEntities) =>
        JsonSerializer.Deserialize<ServerGetBlocksAndEntitiesMessage>(jsonString)!,

      (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.GetPlayerInfo) =>
        JsonSerializer.Deserialize<ClientGetPlayerInfoMessage>(jsonString)!,
      (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.GetPlayerInfo) =>
        JsonSerializer.Deserialize<ServerGetPlayerInfoMessage>(jsonString)!,

      (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.GetTick) =>
        JsonSerializer.Deserialize<ClientGetTickMessage>(jsonString)!,
      (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.GetTick) =>
        JsonSerializer.Deserialize<ServerGetTickMessage>(jsonString)!,


      (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.PerformAttack) =>
        JsonSerializer.Deserialize<ClientPerformAttackMessage>(jsonString)!,

      (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.PerformCraft) =>
        JsonSerializer.Deserialize<ClientPerformCraftMessage>(jsonString)!,

      (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.PerformDropItem) =>
        JsonSerializer.Deserialize<ClientPerformDropItemMessage>(jsonString)!,

      (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.PerformJump) =>
        JsonSerializer.Deserialize<ClientPerformJumpMessage>(jsonString)!,

      (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.PerformLookAt) =>
        JsonSerializer.Deserialize<ClientPerformLookAtMessage>(jsonString)!,

      (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.PerformMergeSlots) =>
        JsonSerializer.Deserialize<ClientPerformMergeSlotsMessage>(jsonString)!,

      (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.PerformMove) =>
        JsonSerializer.Deserialize<ClientPerformMoveMessage>(jsonString)!,

      (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.PerformRotate) =>
        JsonSerializer.Deserialize<ClientPerformRotateMessage>(jsonString)!,

      (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.PerformSwapSlots) =>
        JsonSerializer.Deserialize<ClientPerformSwapSlotsMessage>(jsonString)!,


      (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.PerformSwitchMainHandSlot) =>
        JsonSerializer.Deserialize<ClientPerformSwitchMainHandSlotMessage>(jsonString)!,

      (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.PerformUse) =>
        JsonSerializer.Deserialize<ClientPerformUseMessage>(jsonString)!,

      _ => throw new Exception("The message is not supported"),
    };
  }
}