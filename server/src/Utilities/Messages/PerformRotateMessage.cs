using System.Text.Json.Serialization;

namespace NovelCraft.Utilities.Messages;


public record ClientPerformRotateMessage : MessageBase, IClientMessage {
  [JsonPropertyName("bound_to")]
  public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ServerBound;

  [JsonPropertyName("type")]
  public override IMessage.MessageKind Type => IMessage.MessageKind.PerformRotate;

  [JsonPropertyName("token")]
  public required string Token { get; init; }

  [JsonPropertyName("orientation")]
  public required Orientation Orientation { get; init; }
}