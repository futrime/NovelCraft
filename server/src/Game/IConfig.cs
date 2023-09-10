namespace NovelCraft.Server.Game;

public interface IConfig {
  public int TicksPerSecond { get; }

  public int PlayerSpawnMaxY { get; }
}