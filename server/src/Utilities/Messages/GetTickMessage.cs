using System.Text.Json.Serialization;

namespace NovelCraft.Utilities.Messages;



public record ClientGetTickMessage : MessageBase, IClientMessage {
  [JsonPropertyName("bound_to")]
  public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ServerBound;

  [JsonPropertyName("type")]
  public override IMessage.MessageKind Type => IMessage.MessageKind.GetTick;

  [JsonPropertyName("token")]
  public required string Token { get; init; }
}


public record ServerGetTickMessage : MessageBase {
  [JsonPropertyName("bound_to")]
  public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

  [JsonPropertyName("type")]
  public override IMessage.MessageKind Type => IMessage.MessageKind.GetTick;

  [JsonPropertyName("tick")]
  public required int Tick { get; init; }

  [JsonPropertyName("ticks_per_second")]
  public required decimal TicksPerSecond { get; init; }
}