using System.Text.Json.Serialization;

namespace NovelCraft.Server;

public record Config : Server.IConfig, Game.IConfig {
  [JsonPropertyName("server_port")]
  public required int ServerPort { get; init; } = 14514;

  [JsonPropertyName("level_name")]
  public required string LevelName { get; init; } = "NovelCraft level";

  [JsonPropertyName("ticks_per_second")]
  public required int TicksPerSecond { get; init; } = 20;

  [JsonPropertyName("save_level")]
  public required bool SaveLevel { get; init; } = false;

  [JsonPropertyName("save_record")]
  public required bool SaveRecord { get; init; } = true;

  [JsonPropertyName("player_spawn_max_y")]
  public required int PlayerSpawnMaxY { get; init; } = 128;

  [JsonPropertyName("waiting_time")]
  public required decimal WaitingTime { get; init; } = 0;

  [JsonPropertyName("max_tick")]
  public required int? MaxTick { get; init; } = null;
}