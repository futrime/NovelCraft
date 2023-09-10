using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace NovelCraft.Utilities.Messages;

internal record EmptyMessage : IMessage {
  [JsonPropertyName("bound_to")]
  public IMessage.BoundToKind BoundTo { get; init; }

  [JsonPropertyName("type")]
  public IMessage.MessageKind Type { get; init; }

  [JsonIgnore]
  public JsonNode Json {
    get => JsonNode.Parse(JsonSerializer.Serialize((object)this))!;
  }

  [JsonIgnore]
  public string JsonString {
    get => JsonSerializer.Serialize((object)this);
  }
}