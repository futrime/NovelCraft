using System.Text.Json.Serialization;

namespace NovelCraft.Server.Game;

/// <summary>
/// BlockDefinition class is the definition of a block.
/// </summary>
public record BlockDefinition : IDefinition {
  [JsonPropertyName("type")]
  public string Type => "block";

  [JsonPropertyName("description")]
  public required DescriptionType Description { get; init; }

  [JsonPropertyName("components")]
  public required ComponentsType Components { get; init; }


  public record DescriptionType {
    [JsonPropertyName("identifier")]
    public required string Identifier { get; init; }

    [JsonPropertyName("type_id")]
    public required int TypeId { get; init; }
  }

  public record ComponentsType {
    [JsonPropertyName("collision_box")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public CollisionBoxType? CollisionBox { get; init; } = null;

    [JsonPropertyName("destructible_by_mining")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DestructibleByMiningType? DestructibleByMining { get; init; } = null;

    [JsonPropertyName("friction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Friction { get; init; } = null;


    public record CollisionBoxType { }

    public record DestructibleByMiningType {
      [JsonPropertyName("seconds_to_destroy")]
      public required decimal SecondsToDestroy { get; init; }
    }
  }
}