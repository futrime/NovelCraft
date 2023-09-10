using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace NovelCraft.Utilities.Messages;


public abstract record MessageBase : IMessage {
  [JsonIgnore]
  public JsonNode Json {
    get => JsonNode.Parse(JsonSerializer.Serialize((object)this))!;
  }

  [JsonIgnore]
  public string JsonString {
    get => JsonSerializer.Serialize((object)this);
  }

  [JsonPropertyName("bound_to")]
  public abstract IMessage.BoundToKind BoundTo { get; }

  [JsonPropertyName("type")]
  public abstract IMessage.MessageKind Type { get; }
}
