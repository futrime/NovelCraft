using System.Text.Json.Nodes;

namespace NovelCraft.Utilities.Messages;

/// <summary>
/// Represents common interfaces for all messages.
/// </summary>
public interface IMessage {
  /// <summary>
  /// Represents the transmission direction of the message.
  /// </summary>
  public enum BoundToKind {
    /// <summary>
    /// The message is sent from the client to the server.
    /// </summary>
    ServerBound,

    /// <summary>
    /// The message is sent from the server to the client.
    /// </summary>
    ClientBound
  }

  /// <summary>
  /// Represents the type of the message.
  /// </summary>
  public enum MessageKind {
    Ping = 100,

    Error = 200,

    AfterBlockChange = 400,
    AfterEntityAttack,
    AfterEntityCreate,
    AfterEntityDespawn,
    AfterEntityHurt,
    AfterEntityOrientationChange,
    AfterEntityPositionChange,
    AfterEntityRemove,
    AfterEntitySpawn,
    AfterPlayerInventoryChange,

    GetBlocksAndEntities = 300,
    GetPlayerInfo,
    GetTick,

    PerformAttack = 500,
    PerformCraft,
    PerformDropItem,
    PerformJump,
    PerformMergeSlots,
    PerformMove,
    PerformLookAt,
    PerformRotate,
    PerformSwapSlots,
    PerformSwitchMainHandSlot,
    PerformUse,
  }


  /// <summary>
  /// Gets the JSON representation of the message.
  /// </summary>
  public JsonNode Json { get; }

  /// <summary>
  /// Gets the JSON string representation of the message.
  /// </summary>
  public string JsonString { get; }

  /// <summary>
  /// Gets the transmission direction of the message.
  /// </summary>
  public BoundToKind BoundTo { get; }

  /// <summary>
  /// Gets the type of the message.
  /// </summary>
  public MessageKind Type { get; }
}