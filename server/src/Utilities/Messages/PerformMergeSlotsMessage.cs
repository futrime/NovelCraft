using System.Text.Json.Serialization;

namespace NovelCraft.Utilities.Messages;



public record ClientPerformMergeSlotsMessage : MessageBase, IClientMessage {
  [JsonPropertyName("bound_to")]
  public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ServerBound;

  [JsonPropertyName("type")]
  public override IMessage.MessageKind Type => IMessage.MessageKind.PerformMergeSlots;

  [JsonPropertyName("token")]
  public required string Token { get; init; }

  [JsonPropertyName("from_slot")]
  public required int FromSlot { get; init; }

  [JsonPropertyName("to_slot")]
  public required int ToSlot { get; init; }
}