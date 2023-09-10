namespace NovelCraft.Server.Game;

public class Section {
  #region Fields and properties
  /// <summary>
  /// The position of the section.
  /// </summary>
  /// <remarks>
  /// The position is the position of the minimum corner of the section.
  /// </remarks>
  public Position<int> Position { get; }

  private List<int> _blockIdList;
  #endregion


  #region Constructors and finalizers
  /// <summary>
  /// Creates a new section.
  /// </summary>
  /// <param name="position">
  /// The position of the section. Every dimension should be multiples of 16.
  /// </param>
  /// <param name="blockIdList">The list of block IDs.</param>
  public Section(Position<int> position, List<int> blockIdList) {
    Position = position;

    // Check if the position is valid.
    if (position.X % 16 != 0 || position.Y % 16 != 0 || position.Z % 16 != 0) {
      throw new ArgumentException("The position should be multiples of 16.");
    }

    // Check if the block ID list is right size.
    if (blockIdList.Count != 4096) {
      throw new ArgumentException("The block ID list must have 4096 elements.");
    }

    _blockIdList = new(blockIdList);
  }
  #endregion


  #region Methods
  public ref readonly List<int> GetAllBlockIds() {
    return ref _blockIdList;
  }

  /// <summary>
  /// Gets the block ID by the absolute position.
  /// </summary>
  /// <param name="position">The absolute position.</param>
  public int GetBlockId(Position<int> position) {
    // Check if the position is valid.
    if (position.X < Position.X || position.X >= Position.X + 16
      || position.Y < Position.Y || position.Y >= Position.Y + 16
      || position.Z < Position.Z || position.Z >= Position.Z + 16) {
      throw new ArgumentException("The position is out of range.");
    }

    Position<int> relativePosition = new() {
      X = position.X - this.Position.X,
      Y = position.Y - this.Position.Y,
      Z = position.Z - this.Position.Z
    };

    int blockId = GetBlockIdByRelativePosition(relativePosition);

    return blockId;
  }

  /// <summary>
  /// Gets the block ID by the relative position.
  /// </summary>
  /// <param name="relativePosition">The relative position.</param>
  public int GetBlockIdByRelativePosition(Position<int> relativePosition) {
    // Check if the position is valid.
    if (relativePosition.X < 0 || relativePosition.X >= 16 || relativePosition.Y < 0
      || relativePosition.Y >= 16 || relativePosition.Z < 0 || relativePosition.Z >= 16) {
      throw new ArgumentException("The position is out of range.");
    }

    int blockId = _blockIdList[relativePosition.X * 256 + relativePosition.Y * 16 + relativePosition.Z];

    return blockId;
  }

  /// <summary>
  /// Sets the block ID by the absolute position.
  /// </summary>
  /// <param name="position">The absolute position.</param>
  /// <param name="blockId">The block ID.</param>
  public void SetBlockId(Position<int> position, int blockId) {
    // Check if the position is valid.
    if (position.X < Position.X || position.X >= Position.X + 16
      || position.Y < Position.Y || position.Y >= Position.Y + 16
      || position.Z < Position.Z || position.Z >= Position.Z + 16) {
      throw new ArgumentException("The position is out of range.");
    }

    Position<int> relativePosition = new() {
      X = position.X - this.Position.X,
      Y = position.Y - this.Position.Y,
      Z = position.Z - this.Position.Z
    };

    SetBlockIdByRelativePosition(relativePosition, blockId);
  }

  /// <summary>
  /// Sets the block ID by the relative position.
  /// </summary>
  /// <param name="relativePosition">The relative position.</param>
  /// <param name="blockId">The block ID.</param>
  public void SetBlockIdByRelativePosition(Position<int> relativePosition, int blockId) {
    // Check if the position is valid.
    if (relativePosition.X < 0 || relativePosition.X >= 16 || relativePosition.Y < 0
      || relativePosition.Y >= 16 || relativePosition.Z < 0 || relativePosition.Z >= 16) {
      throw new ArgumentException("The position is out of range.");
    }

    _blockIdList[relativePosition.X * 256 + relativePosition.Y * 16 + relativePosition.Z] = blockId;
  }
  #endregion
}