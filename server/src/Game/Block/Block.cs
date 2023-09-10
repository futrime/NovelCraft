namespace NovelCraft.Server.Game;

public partial class Block {
  #region Fields and properties
  public Position<decimal> Center => new() {
    X = Position.X + 0.5m,
    Y = Position.Y + 0.5m,
    Z = Position.Z + 0.5m
  };

  private string Identifier => _definition.Description.Identifier;

  public Position<int> Position { get; }

  public int TypeId => _definition.Description.TypeId;

  private BlockDefinition _definition;
  #endregion


  #region Constructors and finalizers
  public Block(BlockDefinition definition, Position<int> position) {
    _definition = definition;
    Position = position;
  }
  #endregion


  #region Methods
  #endregion
}