namespace NovelCraft.Server.Game;

public class Velocity : Vector3<decimal> {
  public Velocity() : base() { }

  public Velocity(decimal x, decimal y, decimal z) : base(x, y, z) { }

  public Velocity(Vector3<decimal> vector) : base(vector) { }
}
