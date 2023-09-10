using System.Text.Json.Serialization;

namespace NovelCraft.Utilities.Messages;



internal record ClientPerformSwapSlotsMessage : MessageBase, IClientMessage {
  [JsonPropertyName("bound_to")]
  public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ServerBound;

  [JsonPropertyName("type")]
  public override IMessage.MessageKind Type => IMessage.MessageKind.PerformSwapSlots;

  [JsonPropertyName("token")]
  public required string Token { get; init; }

  [JsonPropertyName("slot_a")]
  public required int SlotA { get; init; }

  [JsonPropertyName("slot_b")]
  public required int SlotB { get; init; }
}