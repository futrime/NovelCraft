namespace NovelCraft.Sdk;

internal class Entity : IEntity {
  public IOrientation Orientation { get; set; } = new Orientation(0, 0);
  public IPosition<decimal> Position { get; set; } = new Position<decimal>(0, 0, 0);
  public int TypeId { get; }
  public int UniqueId { get; }


  public Entity(int uniqueId, int typeId, IPosition<decimal> position, IOrientation orientation) {
    UniqueId = uniqueId;
    TypeId = typeId;
    Position = position;
    Orientation = orientation;
  }
}