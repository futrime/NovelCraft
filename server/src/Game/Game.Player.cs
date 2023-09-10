namespace NovelCraft.Server.Game;

public partial class Game {
  #region Methods
  /// <summary>
  /// Gets a player by its unique ID.
  /// </summary>
  public Player? GetPlayer(int uniqueId) {
    Entity? entity = GetEntity(uniqueId);
    if (entity is Player player) {
      return player;
    }
    return null;
  }

  /// <summary>
  /// Spawns a player.
  /// </summary>
  /// <returns>The unique ID of the player</returns>
  public int CreatePlayer() {
    return CreateEntity(0, GenerateSpawnPosition());
  }
  #endregion
}
