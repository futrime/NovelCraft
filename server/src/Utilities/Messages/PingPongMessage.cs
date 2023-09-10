using System.Text.Json.Serialization;

namespace NovelCraft.Utilities.Messages;


public record ClientPingMessage : MessageBase, IClientMessage {
  [JsonPropertyName("bound_to")]
  public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ServerBound;

  [JsonPropertyName("type")]
  public override IMessage.MessageKind Type => IMessage.MessageKind.Ping;

  [JsonPropertyName("token")]
  public required string Token { get; init; }

  [JsonPropertyName("sent_time")]
  public required decimal SentTime { get; init; }
}

public record ServerPongMessage : MessageBase {
  [JsonPropertyName("bound_to")]
  public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

  [JsonPropertyName("type")]
  public override IMessage.MessageKind Type => IMessage.MessageKind.Ping;

  [JsonPropertyName("sent_time")]
  public required decimal SentTime { get; init; }
}