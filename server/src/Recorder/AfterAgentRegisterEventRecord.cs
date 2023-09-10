using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace NovelCraft.Server.Recorder;

public record AfterAgentRegisterEventRecord : IRecord {
  [JsonPropertyName("identifier")]
  public string Identifier => "after_agent_register";

  [JsonPropertyName("tick")]
  public required int Tick { get; init; }

  [JsonPropertyName("type")]
  public string Type => "event";

  [JsonIgnore]
  public JsonNode Json => JsonNode.Parse(JsonSerializer.Serialize(this))!;

  [JsonPropertyName("data")]
  public required DataType Data { get; init; }

  public record DataType {
    [JsonPropertyName("agent_list")]
    public required List<AgentType> SpawnList { get; init; }
  }

  public record AgentType {
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("token")]
    public required string Token { get; init; }

    [JsonPropertyName("unique_id")]
    public required int UniqueId { get; init; }
  }
}
