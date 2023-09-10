using System.IO.Compression;

namespace NovelCraft.Server.Game;

public partial class Game {
  #region Fields and properties
  private Level _level = new();
  #endregion


  #region Methods
  /// <summary>
  /// Load a level from a zip archive.
  /// </summary>
  public void LoadLevel(ZipArchive levelDataZipArchive) {
    ZipArchiveEntry levelDataEntry = levelDataZipArchive.GetEntry("level_data.json") ??
      throw new Exception("Level data not found in zip archive.");

    // Reads the level data to a JSON object.
    using Stream levelDataEntryStream = levelDataEntry.Open();
    using StreamReader levelDataEntryStreamReader = new StreamReader(levelDataEntryStream);
    string jsonString = levelDataEntryStreamReader.ReadToEnd();

    _level = new Level(LevelData.NewFromJsonStr(jsonString));
  }

  /// <summary>
  /// Save the level to a zip archive.
  /// </summary>
  public void SaveLevel(ZipArchive levelDataZipArchive) {
    // Create the level data entry.
    ZipArchiveEntry levelDataEntry = levelDataZipArchive.CreateEntry("level_data.json");

    // Writes the level data to the entry.
    using Stream levelDataEntryStream = levelDataEntry.Open();
    using StreamWriter levelDataEntryStreamWriter = new StreamWriter(levelDataEntryStream);
    levelDataEntryStreamWriter.Write(_level.Json.ToJsonString());
  }

  /// <summary>
  /// Get section at position
  /// </summary>
  /// <param name="position"></param>
  /// <returns>section</returns>
  public Section GetSectionAt(Position<int> position) {
    return _level.GetSectionAt(position);
  }
  #endregion
}