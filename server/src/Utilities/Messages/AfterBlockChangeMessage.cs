using System.Text.Json.Serialization;

namespace NovelCraft.Utilities.Messages;

public record ServerAfterBlockChangeMessage : MessageBase {
  public record ChangeType {
    [JsonPropertyName("position")]
    public required Position<int> Position { get; init; }

    [JsonPropertyName("block_type_id")]
    public required int BlockTypeId { get; init; }
  }

  
  [JsonPropertyName("bound_to")]
  public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

  [JsonPropertyName("type")]
  public override IMessage.MessageKind Type => IMessage.MessageKind.AfterBlockChange;

  [JsonPropertyName("change_list")]
  public required List<ChangeType> ChangeList { get; init; }
}