using System.Text.Json.Serialization;

namespace NovelCraft.Utilities.Messages;

internal record ServerAfterEntityCreateMessage : MessageBase {
  public record CreationType {
    [JsonPropertyName("entity_type_id")]
    public required int EntityTypeId { get; init; }

    [JsonPropertyName("unique_id")]
    public required int UniqueId { get; init; }

    [JsonPropertyName("position")]
    public required Position<decimal> Position { get; init; }

    [JsonPropertyName("orientation")]
    public required Orientation Orientation { get; init; }

    [JsonPropertyName("item_type_id")]
    public int? ItemTypeId { get; init; } = null;

    [JsonPropertyName("health")]
    public decimal? Health { get; init; } = null;
  }


  [JsonPropertyName("bound_to")]
  public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

  [JsonPropertyName("type")]
  public override IMessage.MessageKind Type => IMessage.MessageKind.AfterEntityCreate;

  [JsonPropertyName("creation_list")]
  public required List<CreationType> CreationList { get; init; }
}