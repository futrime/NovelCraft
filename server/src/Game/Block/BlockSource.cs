namespace NovelCraft.Server.Game;

/// <summary>
/// A BlockSource object contains all sections.
/// </summary>
public class BlockSource {
  #region Static, const and readonly fields
  /// <summary>
  /// The default block type ID.
  /// </summary>
  private const int DefaultBlockTypeId = -1; // Barrier block.
  #endregion


  #region Fields and properties
  private BlockFactory _blockFactory = new();

  private Dictionary<Position<int>, Section> _sectionMap { get; } = new();
  #endregion


  #region Constructors and finalizers
  /// <summary>
  /// Creates a new block source.
  /// </summary>
  /// <param name="sectionList">The list of sections.</param>
  public BlockSource(List<Section> sectionList) {
    foreach (Section section in sectionList) {
      _sectionMap.Add(section.Position, section);
    }
  }

  public BlockSource() {
    // Empty.
  }
  #endregion


  #region Methods
  /// <summary>
  /// Gets all sections.
  /// </summary>
  /// <returns>The list of sections.</returns>
  public List<Section> GetAllSections() {
    return _sectionMap.Values.ToList();
  }

  /// <summary>
  /// Gets the block at the specified position.
  /// </summary>
  public Block GetBlock(Position<int> position) {
    Section section = GetSection(position);
    int blockId = section.GetBlockId(position);
    Block block = _blockFactory.CreateBlock(blockId, position);
    return block;
  }

  /// <summary>
  /// Gets the block position range of the block source.
  /// </summary>
  /// <returns>The block position range.</returns>
  public (Position<int> Min, Position<int> Max) GetBlockPositionRange() {
    Position<int> min = new() { X = int.MaxValue, Y = int.MaxValue, Z = int.MaxValue };
    Position<int> max = new() { X = int.MinValue, Y = int.MinValue, Z = int.MinValue };

    foreach (Section section in _sectionMap.Values) {
      Position<int> sectionPosition = section.Position;

      if (sectionPosition.X < min.X) {
        min.X = sectionPosition.X;
      }

      if (sectionPosition.Y < min.Y) {
        min.Y = sectionPosition.Y;
      }

      if (sectionPosition.Z < min.Z) {
        min.Z = sectionPosition.Z;
      }

      if (sectionPosition.X + 15 > max.X) {
        max.X = sectionPosition.X + 15;
      }

      if (sectionPosition.Y + 15 > max.Y) {
        max.Y = sectionPosition.Y + 15;
      }

      if (sectionPosition.Z + 15 > max.Z) {
        max.Z = sectionPosition.Z + 15;
      }
    }

    return (min, max);
  }

  /// <summary>
  /// Gets the section of the block at the specified position.
  /// </summary>
  public Section GetSection(Position<int> position) {
    Position<int> sectionPosition = new Position<int> {
      X = 16 * (int)Math.Floor(position.X / 16.0),
      Y = 16 * (int)Math.Floor(position.Y / 16.0),
      Z = 16 * (int)Math.Floor(position.Z / 16.0)
    };

    // If the section does not exist, create a new one with default blocks.
    if (!_sectionMap.ContainsKey(sectionPosition)) {
      Section section = new(sectionPosition, new List<int>(Enumerable.Repeat(DefaultBlockTypeId, 4096)));
      return section;
    } else {
      return _sectionMap[sectionPosition];
    }
  }

  /// <summary>
  /// Registers a block definition.
  /// </summary>
  /// <param name="definition">The block definition</param>
  public void RegisterDefinition(BlockDefinition definition) {
    _blockFactory.RegisterDefinition(definition);
  }

  /// <summary>
  /// Registers a list of block definitions.
  /// </summary>
  /// <param name="definitionList">The list of block definitions</param>
  public void RegisterDefinition(List<BlockDefinition> definitionList) {
    _blockFactory.RegisterDefinition(definitionList);
  }

  /// <summary>
  /// Sets the block at the specified position.
  /// </summary>
  /// <param name="position">The position of the block.</param>
  /// <param name="blockId">The block ID.</param>
  public void SetBlock(Position<int> position, int blockId) {
    GetSection(position).SetBlockId(position, blockId);
  }

  /// <summary>
  /// Sets the block at the specified position.
  /// </summary>
  /// <param name="position">The position of the block.</param>
  /// <param name="block">The block.</param>
  public void SetBlock(Position<int> position, Block block) {
    SetBlock(position, block.TypeId);
  }
  #endregion
}
