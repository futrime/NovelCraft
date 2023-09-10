using System.Text.Json.Serialization;

namespace NovelCraft.Server.Game;

public record LootTableDefinition : IDefinition {
  [JsonPropertyName("type")]
  public string Type => "loot_table";

  [JsonPropertyName("description")]
  public required DescriptionType Description { get; init; }

  [JsonPropertyName("pools")]
  public required List<PoolType> Pools { get; init; }

  [JsonPropertyName("only_loot_by")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public List<int>? OnlyLootBy { get; init; } = null;


  public record DescriptionType {
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("block_or_entity_type_id")]
    public required int BlockOrEntityTypeId { get; init; }
  }

  public record PoolType {
    [JsonPropertyName("rolls")]
    public required int Rolls { get; init; }

    [JsonPropertyName("entries")]
    public required List<EntryType> Entries { get; init; }


    public record EntryType {
      [JsonPropertyName("weight")]
      public required int Weight { get; init; }

      [JsonPropertyName("item_type_id")]
      public int? ItemTypeId { get; init; } = null;
    }
  }
}