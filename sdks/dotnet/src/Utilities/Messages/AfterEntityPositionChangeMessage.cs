using System.Text.Json.Serialization;

namespace NovelCraft.Utilities.Messages;

internal record ServerAfterEntityPositionChangeMessage : MessageBase {
  public record ChangeType {
    [JsonPropertyName("unique_id")]
    public required int UniqueId { get; init; }

    [JsonPropertyName("position")]
    public required Position<decimal> Position { get; init; }
  }


  [JsonPropertyName("bound_to")]
  public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

  [JsonPropertyName("type")]
  public override IMessage.MessageKind Type => IMessage.MessageKind.AfterEntityPositionChange;

  [JsonPropertyName("change_list")]
  public required List<ChangeType> ChangeList { get; init; }
}