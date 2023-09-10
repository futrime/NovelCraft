namespace NovelCraft.Sdk {

  /// <summary>
  /// Represents a position.
  /// </summary>
  public interface IPosition<T> {
    /// <summary>
    /// Gets or sets the X coordinate of the position.
    /// </summary>
    public T X { get; set; }

    /// <summary>
    /// Gets or sets the Y coordinate of the position.
    /// </summary>
    public T Y { get; set; }

    /// <summary>
    /// Gets or sets the Z coordinate of the position.
    /// </summary>
    public T Z { get; set; }
  }

}