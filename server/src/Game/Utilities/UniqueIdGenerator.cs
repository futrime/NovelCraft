namespace NovelCraft.Server.Game;

/// <summary>
/// UniqueIdGenerator generates unique IDs.
/// </summary>
public class UniqueIdGenerator {
  #region Fields and properties
  private int _id = 0;
  #endregion


  #region Constructors and finalizers
  public UniqueIdGenerator() {
    // Empty.
  }
  #endregion


  #region Methods
  /// <summary>
  /// Generate method generates a unique ID.
  /// </summary>
  public int Generate() {
    return _id++;
  }
  #endregion
}