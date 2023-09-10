namespace NovelCraft.Server.Game;

public class Acceleration : Vector3<decimal> {
  public Acceleration() : base() { }

  public Acceleration(decimal x, decimal y, decimal z): base(x, y, z) { }

  public Acceleration(Vector3<decimal> vector): base(vector) { }
}
