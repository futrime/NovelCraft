namespace NovelCraft.Server.Game;

public class Position<T> : Vector3<T> where T : struct {
  public Position(): base() { }

  public Position(T x, T y, T z): base(x, y, z) { }

  public Position(Vector3<T> vector): base(vector) { }

  public static implicit operator Position<decimal>(Position<T> position) {
    return new Position<decimal>(
      (decimal)(dynamic)position.X,
      (decimal)(dynamic)position.Y,
      (decimal)(dynamic)position.Z
    );
  }

  public static implicit operator Position<int>(Position<T> position) {
    return new Position<int>(
      (int)(dynamic)position.X,
      (int)(dynamic)position.Y,
      (int)(dynamic)position.Z
    );
  }

  /// <summary>
  /// Gets the distance to another position.
  /// </summary>
  /// <param name="other">The other position</param>
  /// <returns>The distance to the other position</returns>
  public T DistanceTo(Position<T> other) {
    return DistanceBetween(this, other);
  }

  /// <summary>
  /// Gets the distance between two positions.
  /// </summary>
  /// <param name="a">The first position</param>
  /// <param name="b">The second position</param>
  /// <returns>The distance between the two positions</returns>
  public static T DistanceBetween(Position<T> a, Position<T> b) {
    return (a - b).Length;
  }
}
