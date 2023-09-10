using System.Text.Json.Serialization;

namespace NovelCraft.Utilities.Messages;



internal record ClientPerformJumpMessage : MessageBase, IClientMessage {
  [JsonPropertyName("bound_to")]
  public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ServerBound;

  [JsonPropertyName("type")]
  public override IMessage.MessageKind Type => IMessage.MessageKind.PerformJump;

  [JsonPropertyName("token")]
  public required string Token { get; init; }
}