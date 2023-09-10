using System.Text.Json.Serialization;

namespace NovelCraft.Utilities.Messages;

internal record ServerAfterEntityOrientationChangeMessage : MessageBase {
  public record ChangeType {
    [JsonPropertyName("unique_id")]
    public required int UniqueId { get; init; }

    [JsonPropertyName("orientation")]
    public required Orientation Orientation { get; init; }
  }


  [JsonPropertyName("bound_to")]
  public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

  [JsonPropertyName("type")]
  public override IMessage.MessageKind Type => IMessage.MessageKind.AfterEntityOrientationChange;

  [JsonPropertyName("change_list")]
  public required List<ChangeType> ChangeList { get; init; }
}