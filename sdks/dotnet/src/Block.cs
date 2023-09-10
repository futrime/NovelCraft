namespace NovelCraft.Sdk;

internal class Block: IBlock {
  public IPosition<int> Position => _position;
  public int TypeId { get; }

  private Position<int> _position;


  public Block(int typeId, Position<int> position) {
    TypeId = typeId;
    _position = position;
  }
}