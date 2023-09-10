using System.Text.Json.Serialization;

namespace NovelCraft.Server.Game;

public record RecipeDefinition : IDefinition {
  [JsonPropertyName("type")]
  public string Type => "recipe";

  [JsonPropertyName("recipe")]
  public required RecipeType Recipe { get; init; }


  public record RecipeType {
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("ingredients")]
    public required List<int?> Ingredients { get; init; }

    [JsonPropertyName("result")]
    public required List<ResultType> Result { get; init; }

    [JsonPropertyName("is_shapeless")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsShapeless { get; init; }


    public record ResultType {
      [JsonPropertyName("item_type_id")]
      public required int ItemTypeId { get; init; }

      [JsonPropertyName("count")]
      public required int Count { get; init; }
    }
  }
}