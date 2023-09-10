namespace NovelCraft.Server.Game;

public class AABB {
  public Position<decimal> Min { get; }

  public Position<decimal> Max { get; }

  public SizeType<decimal> Size => new SizeType<decimal>(Max.X - Min.X, Max.Y - Min.Y, Max.Z - Min.Z);
  
  /// <summary>
  /// Returns the vertexes of the AABB in the following order:
  /// 0: Min.X, Min.Y, Min.Z
  /// 1: Min.X, Min.Y, Max.Z
  /// 2: Min.X, Max.Y, Min.Z
  /// 3: Min.X, Max.Y, Max.Z
  /// 4: Max.X, Min.Y, Min.Z
  /// 5: Max.X, Min.Y, Max.Z
  /// 6: Max.X, Max.Y, Min.Z
  /// 7: Max.X, Max.Y, Max.Z
  /// </summary>
  public List<Position<decimal>> Vertices {
    get {
      return new List<Position<decimal>> {
        new Position<decimal>(Min.X, Min.Y, Min.Z),
        new Position<decimal>(Min.X, Min.Y, Max.Z),
        new Position<decimal>(Min.X, Max.Y, Min.Z),
        new Position<decimal>(Min.X, Max.Y, Max.Z),
        new Position<decimal>(Max.X, Min.Y, Min.Z),
        new Position<decimal>(Max.X, Min.Y, Max.Z),
        new Position<decimal>(Max.X, Max.Y, Min.Z),
        new Position<decimal>(Max.X, Max.Y, Max.Z)
      };
    }
  }

  public AABB(Position<decimal> min, Position<decimal> max) {
    // Check that min is less than max
    if (min.X > max.X || min.Y > max.Y || min.Z > max.Z) {
      throw new ArgumentException("Min must be less than max");
    }

    Min = new(min);
    Max = new(max);
  }

  public AABB(Position<decimal> position, SizeType<decimal> size) : this(position, new Position<decimal>(position + size)) { }

  public AABB(AABB aabb) : this(aabb.Min, aabb.Max) { }

  /// <summary>
  /// Returns true if two AABBs intersect.
  /// </summary>
  public static bool Intersect(AABB a, AABB b) {
    // Just contacting should not be considered an intersection.
    return a.Min.X < b.Max.X && a.Max.X > b.Min.X &&
           a.Min.Y < b.Max.Y && a.Max.Y > b.Min.Y &&
           a.Min.Z < b.Max.Z && a.Max.Z > b.Min.Z;
  }
}
