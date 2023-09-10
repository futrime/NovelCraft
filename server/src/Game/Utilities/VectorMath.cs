namespace NovelCraft.Server.Game;

/// <summary>
/// Provides methods for vector math.
/// </summary>
public static class VectorMath {
  /// <summary>
  /// Gets all points that has a least one integer coordinate in common with the line segment defined by the two given positions.
  /// </summary>
  /// <remarks>
  /// The points are sorted by distance from the first position.
  /// </remarks>
  public static List<Position<decimal>> GetIntersectionPointList(Position<decimal> fromPosition, Position<decimal> toPosition) {
    var list = new List<Position<decimal>>();

    Vector3<decimal> diffPosition = toPosition - fromPosition;

    if (diffPosition.Length == 0) {
      if (fromPosition.X == (int)fromPosition.X ||
          fromPosition.Y == (int)fromPosition.Y ||
          fromPosition.Z == (int)fromPosition.Z) {
        list.Add(new(fromPosition));
      }

      return list;
    }

    Vector3<decimal> unitDiffPosition = diffPosition / diffPosition.Length;

    // Search x-axis
    if (unitDiffPosition.X != 0) {
      (int minX, int maxX) = ((int)Math.Ceiling(Math.Min(fromPosition.X, toPosition.X)), (int)Math.Floor(Math.Max(fromPosition.X, toPosition.X)));
      for (int x = minX; x <= maxX; x++) {
        decimal t = (x - fromPosition.X) / unitDiffPosition.X;

        decimal y = fromPosition.Y + unitDiffPosition.Y * t;
        decimal z = fromPosition.Z + unitDiffPosition.Z * t;
        list.Add(new Position<decimal>(x, y, z));
      }
    }

    // Search y-axis
    if (unitDiffPosition.Y != 0) {
      (int minY, int maxY) = ((int)Math.Ceiling(Math.Min(fromPosition.Y, toPosition.Y)), (int)Math.Floor(Math.Max(fromPosition.Y, toPosition.Y)));
      for (int y = minY; y <= maxY; y++) {
        decimal t = (y - fromPosition.Y) / unitDiffPosition.Y;

        decimal x = fromPosition.X + unitDiffPosition.X * t;
        decimal z = fromPosition.Z + unitDiffPosition.Z * t;
        list.Add(new Position<decimal>(x, y, z));
      }
    }

    // Search z-axis
    if (unitDiffPosition.Z != 0) {
      (int minZ, int maxZ) = ((int)Math.Ceiling(Math.Min(fromPosition.Z, toPosition.Z)), (int)Math.Floor(Math.Max(fromPosition.Z, toPosition.Z)));
      for (int z = minZ; z <= maxZ; z++) {
        decimal t = (z - fromPosition.Z) / unitDiffPosition.Z;

        decimal x = fromPosition.X + unitDiffPosition.X * t;
        decimal y = fromPosition.Y + unitDiffPosition.Y * t;
        list.Add(new Position<decimal>(x, y, z));
      }
    }

    // Sort by distance from fromPosition
    list.Sort((a, b) => {
      decimal aDistance = (a - fromPosition).Length;
      decimal bDistance = (b - fromPosition).Length;
      return aDistance.CompareTo(bDistance);
    });

    // Remove duplicates
    for (int i = 0; i < list.Count - 1; i++) {
      if (list[i].Length == list[i + 1].Length) {
        list.RemoveAt(i);
        i--;
      }
    }

    return list;
  }
}
