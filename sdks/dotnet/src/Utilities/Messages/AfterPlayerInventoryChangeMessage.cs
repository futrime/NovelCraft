using System.Text.Json.Serialization;

namespace NovelCraft.Utilities.Messages;

internal record ServerAfterPlayerInventoryChangeMessage : MessageBase {
  public record ChangeType {
    [JsonPropertyName("player_unique_id")]
    public required int PlayerUniqueId { get; init; }

    [JsonPropertyName("change_list")]
    public required List<InventoryChangeType> ChangeList { get; init; }

    public record InventoryChangeType {
      [JsonPropertyName("slot")]
      public required int Slot { get; init; }

      [JsonPropertyName("count")]
      public required int Count { get; init; }

      [JsonPropertyName("item_type_id")]
      [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
      public int? ItemTypeId { get; init; }
    }
  }


  [JsonPropertyName("bound_to")]
  public override IMessage.BoundToKind BoundTo => IMessage.BoundToKind.ClientBound;

  [JsonPropertyName("type")]
  public override IMessage.MessageKind Type => IMessage.MessageKind.AfterPlayerInventoryChange;

  [JsonPropertyName("change_list")]
  public required List<ChangeType> ChangeList { get; init; }
}