using System.Text.Json.Serialization;

namespace NovelCraft.Utilities.Messages;

public record ServerAfterPlayerSwitchMainHandMessage : MessageBase {
  public record ChangeType {
    [JsonPropertyName("player_unique_id")]
    public required int PlayerUniqueId { get; init; }

    [JsonPropertyName("new_main_hand")]
    public required int NewMainHand { get; init; }
  }


  [JsonPropertyName("bound_to")]
  public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

  [JsonPropertyName("type")]
  public override IMessage.MessageKind Type => IMessage.MessageKind.AfterPlayerSwitchMainHand;

  [JsonPropertyName("change_list")]
  public required List<ChangeType> ChangeList { get; init; }
}
