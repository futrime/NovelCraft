using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace NovelCraft.Server.Recorder;

/// <summary>
/// IRecord declares common interfaces of all records.
/// </summary>
public interface IRecord {
  /// <summary>
  /// Gets the identifier of the record type.
  /// </summary>
  public string Identifier { get; }

  /// <summary>
  /// Gets the tick when the record was created.
  /// </summary>
  public int Tick { get; init; }

  /// <summary>
  /// Gets the type of the record.
  /// </summary>
  public string Type { get; }

  /// <summary>
  /// Gets the JSON representation of the record.
  /// </summary>
  public JsonNode Json { get; }
}