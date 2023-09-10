namespace NovelCraft.Sdk;

internal class Section {
  public IBlock this[IPosition<int> relativePosition] {
    get {
      // Check if the position is valid.
      if (relativePosition.X < 0 || relativePosition.X > 15 ||
            relativePosition.Y < 0 || relativePosition.Y > 15 ||
            relativePosition.Z < 0 || relativePosition.Z > 15) {
        throw new ArgumentException("The position should be in range [0, 15].");
      }

      return new Block(
        _blockIdList[relativePosition.X * 256 + relativePosition.Y * 16 + relativePosition.Z],
        new Position<int>(Position.X + relativePosition.X, Position.Y + relativePosition.Y, Position.Z + relativePosition.Z)
        );
    }

    set {
      // Check if the position is valid.
      if (relativePosition.X < 0 || relativePosition.X > 15 ||
            relativePosition.Y < 0 || relativePosition.Y > 15 ||
            relativePosition.Z < 0 || relativePosition.Z > 15) {
        throw new ArgumentException("The position should be in range [0, 15].");
      }

      _blockIdList[relativePosition.X * 256 + relativePosition.Y * 16 + relativePosition.Z] = value.TypeId;
    }
  }

  public IPosition<int> Position { get; }

  private List<int> _blockIdList;


  public Section(IPosition<int> position, List<int> blockIdList) {
    // Check if the position is valid.
    if (position.X % 16 != 0 || position.Y % 16 != 0 || position.Z % 16 != 0) {
      throw new ArgumentException("The position should be multiples of 16.");
    }

    // Check if the block ID list is right size.
    if (blockIdList.Count != 4096) {
      throw new ArgumentException("The block ID list must have 4096 elements.");
    }

    Position = new Position<int>(position);
    _blockIdList = new(blockIdList);
  }
}