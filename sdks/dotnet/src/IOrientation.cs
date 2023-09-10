namespace NovelCraft.Sdk;

/// <summary>
/// Represents an orientation.
/// </summary>
public interface IOrientation {
  /// <summary>
  /// Gets or sets the yaw of the orientation.
  /// </summary>
  public decimal Yaw { get; set; }

  /// <summary>
  /// Gets or sets the pitch of the orientation.
  /// </summary>
  public decimal Pitch { get; set; }
}