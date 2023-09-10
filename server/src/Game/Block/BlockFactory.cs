namespace NovelCraft.Server.Game;

/// <summary>
/// BlockFactory creates block objects and bind definitions to them.
/// </summary>
public class BlockFactory {
  #region Static, const and readonly fields
  private const int UnknownBlockTypeId = -2;
  #endregion


  #region Fields and properties
  private Dictionary<int, BlockDefinition> _definitionMap = new(); // Block type ID -> Block definition
  #endregion


  #region Constructors and finalizers
  public BlockFactory(List<BlockDefinition> definitionList) {
    foreach (BlockDefinition definition in definitionList) {
      _definitionMap.Add(definition.Description.TypeId, definition);
    }
  }

  public BlockFactory() {
    // Empty.
  }
  #endregion


  #region Methods
  /// <summary>
  /// Creates a new block.
  /// </summary>
  /// <param name="blockTypeId">The block type ID</param>
  /// <param name="position">The position of the block</param>
  /// <returns>The block object</returns>
  public Block CreateBlock(int blockTypeId, Position<int> position) {
    if (!_definitionMap.ContainsKey(blockTypeId)) {
      blockTypeId = UnknownBlockTypeId;
    }

    Block block = new Block(_definitionMap[blockTypeId], position);
    return block;
  }

  /// <summary>
  /// Registers a block definition.
  /// </summary>
  /// <param name="definition">The block definition</param>
  public void RegisterDefinition(BlockDefinition definition) {
    _definitionMap.Add(definition.Description.TypeId, definition);
  }

  /// <summary>
  /// Registers a list of block definitions.
  /// </summary>
  /// <param name="definitionList">The list of block definitions</param>
  public void RegisterDefinition(List<BlockDefinition> definitionList) {
    foreach (BlockDefinition definition in definitionList) {
      RegisterDefinition(definition);
    }
  }
  #endregion
}