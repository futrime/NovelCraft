using System.Text.Json.Serialization;

namespace NovelCraft.Server.Game;

/// <summary>
/// ItemDefinition class is the class that represents an item definition.
/// </summary>
public record ItemDefinition : IDefinition {
  [JsonPropertyName("type")]
  public string Type => "item";

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
    [JsonPropertyName("digger")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DiggerType? Digger { get; init; } = null;

    [JsonPropertyName("food")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public FoodType? Food { get; init; } = null;

    [JsonPropertyName("max_stack_size")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MaxStackSize { get; init; } = null;

    [JsonPropertyName("weapon")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public WeaponType? Weapon { get; init; } = null;

    [JsonPropertyName("block_placer")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public BlockPlacerType? BlockPlacer { get; init; } = null;


    public record BlockPlacerType {
      [JsonPropertyName("block_type_id")]
      public required int BlockTypeId { get; init; }

      [JsonPropertyName("can_place_on")]
      [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
      public List<PlacementType>? CanPlaceOn { get; init; }


      public record PlacementType {
        [JsonPropertyName("kind")]
        public required string Kind { get; init; }

        [JsonPropertyName("block_type_id")]
        public required int BlockTypeId { get; init; }
      }
    }

    public record DiggerType {
      [JsonPropertyName("destroy_speeds")]
      public required List<DestroySpeedType> DestroySpeeds { get; init; }


      public record DestroySpeedType {
        [JsonPropertyName("block_type_id")]
        public required int BlockTypeId { get; init; }

        [JsonPropertyName("speed_multiplier")]
        public required decimal SpeedMultiplier { get; init; }
      }
    }

    public record FoodType {
      [JsonPropertyName("can_always_eat")]
      public required bool CanAlwaysEat { get; init; }

      [JsonPropertyName("nutrition")]
      public required decimal Nutrition { get; init; }
    }

    public record WeaponType {
      [JsonPropertyName("damage")]
      public required decimal Damage { get; init; }
    }
  }
}