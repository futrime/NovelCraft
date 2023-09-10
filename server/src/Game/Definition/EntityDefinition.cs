using System.Text.Json.Serialization;

namespace NovelCraft.Server.Game;

/// <summary>
/// EntityDefinition class is the class that represents an entity definition.
/// </summary>
public record EntityDefinition : IDefinition {
  [JsonPropertyName("type")]
  public string Type => "entity";

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
    [JsonPropertyName("attack")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AttackType? Attack { get; init; } = null;

    [JsonPropertyName("collision_box")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public CollisionBoxType? CollisionBox { get; init; } = null;

    [JsonPropertyName("health")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public HealthType? Health { get; init; } = null;

    [JsonPropertyName("movement")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public MovementType? Movement { get; init; } = null;

    [JsonPropertyName("movement.jump")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public MovementJumpType? MovementJump { get; init; } = null;

    [JsonPropertyName("physics")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public PhysicsType? Physics { get; init; } = null;


    public record AttackType {
      [JsonPropertyName("damage")]
      public required decimal Damage { get; init; }
    }

    public record CollisionBoxType {
      [JsonPropertyName("height")]
      public required decimal Height { get; init; }

      [JsonPropertyName("width")]
      public required decimal Width { get; init; }

      [JsonPropertyName("eye_height")]
      public required decimal EyeHeight { get; init; }
    }

    public record HealthType {
      [JsonPropertyName("value")]
      public required decimal Value { get; init; }

      [JsonPropertyName("max")]
      public required decimal Max { get; init; }
    }

    public record MovementType {
      [JsonPropertyName("value")]
      public required decimal Value { get; init; }
    }

    public record MovementJumpType {
      [JsonPropertyName("value")]
      public required decimal Value { get; init; }
    }

    public record PhysicsType { }
  }
}