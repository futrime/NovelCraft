using System.Text.Json.Nodes;

namespace NovelCraft.Server.Recorder;

public interface IRecorder {
  /// <summary>
  /// Gets the JSON representation of all records in the recorder.
  /// </summary>
  public JsonNode Json { get; }

  /// <summary>
  /// Records a record.
  /// </summary>
  public void Record(IRecord record);

  /// <summary>
  /// Saves all records to a file.
  /// </summary>
  public void Save();
}