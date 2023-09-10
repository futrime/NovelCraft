namespace NovelCraft.Server.Game;

public class SizeType<T> : Vector3<T>
  where T : struct, IComparable<T> {
  public override T X {
    set {
      if (value.CompareTo(default(T)) < 0) {
        throw new ArgumentOutOfRangeException(
          nameof(value),
          value,
          "X cannot be less than zero."
        );
      }

      base.X = value;
    }
  }

  public override T Y {
    set {
      if (value.CompareTo(default(T)) < 0) {
        throw new ArgumentOutOfRangeException(
          nameof(value),
          value,
          "Y cannot be less than zero."
        );
      }

      base.Y = value;
    }
  }

  public override T Z {
    set {
      if (value.CompareTo(default(T)) < 0) {
        throw new ArgumentOutOfRangeException(
          nameof(value),
          value,
          "Z cannot be less than zero."
        );
      }

      base.Z = value;
    }
  }

  public SizeType() : base() { }
  
  public SizeType(T x, T y, T z) : base(x, y, z) { }

  public SizeType(Vector3<T> vector) : base(vector) { }
}
