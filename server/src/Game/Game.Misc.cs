using NovelCraft.Utilities.Logger;

namespace NovelCraft.Server.Game;

public partial class Game {
  private Random _random = new();
  private Logger _logger = new("Server.Game");

  private Position<decimal> GenerateSpawnPosition() {
    const int maxVariation = 8;

    (Position<int> minPosition, Position<int> maxPosition) = _level.GetBlockPositionRange();
    Position<int> midPosition = new((minPosition.X + maxPosition.X) / 2, (minPosition.Y + maxPosition.Y) / 2, (minPosition.Z + maxPosition.Z) / 2);

    minPosition = new Position<int>(Math.Max(minPosition.X, midPosition.X - maxVariation), minPosition.Y, Math.Max(minPosition.Z, midPosition.Z - maxVariation));
    maxPosition = new Position<int>(Math.Min(maxPosition.X, midPosition.X + maxVariation), maxPosition.Y, Math.Min(maxPosition.Z, midPosition.Z + maxVariation));

    bool findProperSpawnPosition = false;
    int x = 0, y = 0, z = 0;

    const int MaxSpawnCount = 64;
    int SpawnCount = 0;
    while (findProperSpawnPosition == false && SpawnCount < MaxSpawnCount) {
      x = this._random.Next(minPosition.X, maxPosition.X);
      z = this._random.Next(minPosition.Z, maxPosition.Z);
      for (y = Math.Min(Config.PlayerSpawnMaxY, maxPosition.Y) - 1; y > minPosition.Y; y--) {
        Block block = _level.GetBlock(new Position<int>(x, y, z));
        if (block != null && block.TypeId != EmptyBlockTypeId) {
          findProperSpawnPosition = true;
          break;
        }
      }
      SpawnCount++;
    }

    if (findProperSpawnPosition == false) {
      throw new Exception("Cannot find proper spawn position.");
    }

    return new Position<decimal>(x + 0.5m, y + 1, z + 0.5m);
  }
}
