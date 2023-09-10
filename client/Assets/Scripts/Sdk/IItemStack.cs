namespace NovelCraft.Sdk {

  /// <summary>
  /// Represents an item stack.
  /// </summary>
  public interface IItemStack {
    /// <summary>
    /// Gets the count of the item stack.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Gets the type of the item stack.
    /// </summary>
    public int TypeId { get; }
  }

}