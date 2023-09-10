using System.Text.Json.Serialization;

namespace NovelCraft.Utilities.Messages;



internal record ClientGetBlocksAndEntitiesMessage : MessageBase, IClientMessage {
  [JsonPropertyName("bound_to")]
  public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ServerBound;

  [JsonPropertyName("type")]
  public override IMessage.MessageKind Type => IMessage.MessageKind.GetBlocksAndEntities;

  [JsonPropertyName("token")]
  public required string Token { get; init; }

  [JsonPropertyName("request_section_list")]
  public required List<Position<int>> RequestSectionList { get; init; }
}


internal record ServerGetBlocksAndEntitiesMessage : MessageBase {
  public record SectionType {
    [JsonPropertyName("position")]
    public required Position<int> Position { get; init; }

    [JsonPropertyName("blocks")]
    public required List<int> Blocks { get; init; }
  }

  public record EntityType {
    [JsonPropertyName("type_id")]
    public required int TypeId { get; init; }

    [JsonPropertyName("unique_id")]
    public required int UniqueId { get; init; }

    [JsonPropertyName("position")]
    public required Position<decimal> Position { get; init; }

    [JsonPropertyName("orientation")]
    public required Orientation Orientation { get; init; }
  }


  [JsonPropertyName("bound_to")]
  public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

  [JsonPropertyName("type")]
  public override IMessage.MessageKind Type => IMessage.MessageKind.GetBlocksAndEntities;

  [JsonPropertyName("sections")]
  public required List<SectionType> Sections { get; init; }

  [JsonPropertyName("entities")]
  public required List<EntityType> Entities { get; init; }
}