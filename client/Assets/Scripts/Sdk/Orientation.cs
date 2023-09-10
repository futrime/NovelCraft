namespace NovelCraft.Sdk {

  /// <summary>
  /// The orientation of an object.
  /// </summary>
  public struct Orientation : IOrientation {
    public decimal Yaw { get; set; }
    public decimal Pitch { get; set; }

    /// <summary>
    /// Creates from an existing orientation.
    /// </summary>
    public Orientation(IOrientation orientation) {
      Yaw = orientation.Yaw;
      Pitch = orientation.Pitch;
    }

    /// <summary>
    /// Creates from yaw and pitch.
    /// </summary>
    public Orientation(decimal yaw, decimal pitch) {
      Yaw = yaw;
      Pitch = pitch;
    }
  }

}