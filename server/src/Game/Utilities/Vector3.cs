namespace NovelCraft.Server.Game;

public class Vector3<T> : IEquatable<Vector3<T>> where T : struct {

  public virtual T X {
    get => _x;
    set => _x = value;
  }

  public virtual T Y {
    get => _y;
    set => _y = value;
  }

  public virtual T Z {
    get => _z;
    set => _z = value;
  }

  public virtual T Length {
    get {
      return (T)(dynamic)Math.Sqrt(
        (double)(dynamic)X * (double)(dynamic)X +
        (double)(dynamic)Y * (double)(dynamic)Y +
        (double)(dynamic)Z * (double)(dynamic)Z
      );
    }
  }

  protected T _x;
  protected T _y;
  protected T _z;

  public Vector3() { }

  public Vector3(T x, T y, T z) {
    X = x;
    Y = y;
    Z = z;
  }

  public Vector3(Vector3<T> vector) {
    X = vector.X;
    Y = vector.Y;
    Z = vector.Z;
  }

  public override bool Equals(object? obj) {
    if (obj is null || GetType() != obj.GetType()) {
      return false;
    }

    return this == (Vector3<T>)obj;
  }

  public override int GetHashCode() {
    return HashCode.Combine(X, Y, Z);
  }

  public override string ToString() {
    return $"({X:F6}, {Y:F6}, {Z:F6})";
  }

  public bool Equals(Vector3<T>? other) {
    if (other is null) {
      return false;
    }

    return this == other;
  }

  public static bool operator ==(Vector3<T> a, Vector3<T> b) {
    return a.X.Equals(b.X) && a.Y.Equals(b.Y) && a.Z.Equals(b.Z);
  }

  public static bool operator !=(Vector3<T> a, Vector3<T> b) {
    return !(a == b);
  }

  public static Vector3<T> operator -(Vector3<T> a) {
    return new Vector3<T>(
      (T)(dynamic)(-(decimal)(dynamic)a.X),
      (T)(dynamic)(-(decimal)(dynamic)a.Y),
      (T)(dynamic)(-(decimal)(dynamic)a.Z)
    );
  }

  public static Vector3<T> operator +(Vector3<T> a, Vector3<T> b) {
    return new Vector3<T>(
      (T)(dynamic)((decimal)(dynamic)a.X + (decimal)(dynamic)b.X),
      (T)(dynamic)((decimal)(dynamic)a.Y + (decimal)(dynamic)b.Y),
      (T)(dynamic)((decimal)(dynamic)a.Z + (decimal)(dynamic)b.Z)
    );
  }

  public static Vector3<T> operator -(Vector3<T> a, Vector3<T> b) {
    return new Vector3<T>(
      (T)(dynamic)((decimal)(dynamic)a.X - (decimal)(dynamic)b.X),
      (T)(dynamic)((decimal)(dynamic)a.Y - (decimal)(dynamic)b.Y),
      (T)(dynamic)((decimal)(dynamic)a.Z - (decimal)(dynamic)b.Z)
    );
  }

  public static Vector3<T> operator *(Vector3<T> a, T b) {
    return new Vector3<T>(
      (T)(dynamic)((decimal)(dynamic)a.X * (decimal)(dynamic)b),
      (T)(dynamic)((decimal)(dynamic)a.Y * (decimal)(dynamic)b),
      (T)(dynamic)((decimal)(dynamic)a.Z * (decimal)(dynamic)b)
    );
  }

  public static Vector3<T> operator *(T a, Vector3<T> b) {
    return new Vector3<T>(
      (T)(dynamic)((decimal)(dynamic)a * (decimal)(dynamic)b.X),
      (T)(dynamic)((decimal)(dynamic)a * (decimal)(dynamic)b.Y),
      (T)(dynamic)((decimal)(dynamic)a * (decimal)(dynamic)b.Z)
    );
  }

  public static Vector3<T> operator /(Vector3<T> a, T b) {
    return new Vector3<T>(
      (T)(dynamic)((decimal)(dynamic)a.X / (decimal)(dynamic)b),
      (T)(dynamic)((decimal)(dynamic)a.Y / (decimal)(dynamic)b),
      (T)(dynamic)((decimal)(dynamic)a.Z / (decimal)(dynamic)b)
    );
  }

  /// <summary>
  /// Returns the component-wise division of a scalar by a vector.
  /// </summary>
  public static Vector3<T> operator /(T a, Vector3<T> b) {
    return new Vector3<T>(
      (T)(dynamic)((decimal)(dynamic)a / (decimal)(dynamic)b.X),
      (T)(dynamic)((decimal)(dynamic)a / (decimal)(dynamic)b.Y),
      (T)(dynamic)((decimal)(dynamic)a / (decimal)(dynamic)b.Z)
    );
  }

  /// <summary>
  /// Returns the dot product of two vectors.
  /// </summary>
  public static T operator *(Vector3<T> a, Vector3<T> b) {
    return (T)(dynamic)(
      (decimal)(dynamic)a.X * (decimal)(dynamic)b.X +
      (decimal)(dynamic)a.Y * (decimal)(dynamic)b.Y +
      (decimal)(dynamic)a.Z * (decimal)(dynamic)b.Z
    );
  }
}
