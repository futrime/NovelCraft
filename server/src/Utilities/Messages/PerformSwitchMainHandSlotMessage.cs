using System.Text.Json.Serialization;

namespace NovelCraft.Utilities.Messages;



public record ClientPerformSwitchMainHandSlotMessage : MessageBase, IClientMessage {
  [JsonPropertyName("bound_to")]
  public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ServerBound;

  [JsonPropertyName("type")]
  public override IMessage.MessageKind Type => IMessage.MessageKind.PerformSwitchMainHandSlot;

  [JsonPropertyName("token")]
  public required string Token { get; init; }

  [JsonPropertyName("new_main_hand")]
  public required int NewMainHand { get; init; }
}