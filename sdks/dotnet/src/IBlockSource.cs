namespace NovelCraft.Sdk;

/// <summary>
/// Represents a collection of blocks.
/// </summary>
public interface IBlockSource {
  /// <summary>
  /// Gets the block at the specified position.
  /// </summary>
  IBlock? this[IPosition<int> position] { get; }

  /// <summary>
  /// Gets the count of blocks loaded.
  /// </summary>
  int Count { get; }
}