namespace NovelCraft.Sdk {

  /// <summary>
  /// A position in a three-dimensional space.
  /// </summary>
  public struct Position<T> : IPosition<T> {
    public T X { get; set; }
    public T Y { get; set; }
    public T Z { get; set; }

    /// <summary>
    /// Creates from an existing position.
    /// </summary>
    public Position(IPosition<T> position) {
      X = position.X;
      Y = position.Y;
      Z = position.Z;
    }

    /// <summary>
    /// Creates from x, y and z.
    /// </summary>
    public Position(T x, T y, T z) {
      X = x;
      Y = y;
      Z = z;
    }
  }

}