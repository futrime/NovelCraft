using System.Text.Json.Serialization;

namespace NovelCraft.Utilities.Messages;



public record ClientPerformLookAtMessage : MessageBase, IClientMessage {
  [JsonPropertyName("bound_to")]
  public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ServerBound;

  [JsonPropertyName("type")]
  public override IMessage.MessageKind Type => IMessage.MessageKind.PerformLookAt;

  [JsonPropertyName("token")]
  public required string Token { get; init; }

  [JsonPropertyName("look_at_position")]
  public required Position<decimal> LookAtPosition { get; init; }
}