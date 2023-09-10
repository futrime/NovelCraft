using System.Text.Json;
using System.Text.Json.Nodes;

namespace NovelCraft.Server.Game;

public interface IDefinition {
  public abstract string Type { get; }


  public static IDefinition NewFromJsonStr(string jsonString) {
    JsonNode definitionJson = JsonNode.Parse(jsonString)!;

    return (string)definitionJson["type"]! switch {
      "block" => JsonSerializer.Deserialize<BlockDefinition>(jsonString)!,
      "entity" => JsonSerializer.Deserialize<EntityDefinition>(jsonString)!,
      "item" => JsonSerializer.Deserialize<ItemDefinition>(jsonString)!,
      "loot_table" => JsonSerializer.Deserialize<LootTableDefinition>(jsonString)!,
      "recipe" => JsonSerializer.Deserialize<RecipeDefinition>(jsonString)!,
      _ => throw new Exception("Invalid definition type."),
    };
  }
}