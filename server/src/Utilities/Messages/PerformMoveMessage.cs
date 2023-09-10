using System.Text.Json.Serialization;
namespace NovelCraft.Utilities.Messages;


public record ClientPerformMoveMessage : MessageBase, IClientMessage {
  [JsonPropertyName("bound_to")]
  public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ServerBound;

  [JsonPropertyName("type")]
  public override IMessage.MessageKind Type => IMessage.MessageKind.PerformMove;

  [JsonPropertyName("token")]
  public required string Token { get; init; }
  public enum Direction {
    Stop,
    Forward,
    Backward,
    Left,
    Right
  }
  [JsonPropertyName("direction")]
  public required Direction DirectionType { get; init; }
}