namespace NovelCraft.Server.Game;

public struct Orientation {
  public decimal Yaw {
    get => _yaw;
    set {
      // Value must be in range (-180, 180].
      if (value <= -180 || value > 180) {
        throw new ArgumentOutOfRangeException(
          nameof(value),
          value,
          "Value must be in range (-180, 180]."
        );
      }

      _yaw = value;
    }
  }
  public decimal Pitch {
    get => _pitch;
    set {
      // Value must be in range [-90, 90].
      if (value < -90 || value > 90) {
        throw new ArgumentOutOfRangeException(
          nameof(value),
          value,
          "Value must be in range [-90, 90]."
        );
      }

      _pitch = value;
    }
  }

  private decimal _yaw;
  private decimal _pitch;

  public Orientation(decimal yaw, decimal pitch) {
    Yaw = yaw;
    Pitch = pitch;
  }
}