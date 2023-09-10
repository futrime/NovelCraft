namespace NovelCraft.Utilities.Messages {


  /// <summary>
  /// Represents common interfaces for all error messages.
  /// </summary>
  public interface IErrorMessage : IMessage {
    /// <summary>
    /// Gets the error code.
    /// </summary>
    public int Code { get; init; }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    public string Message { get; init; }
  }
}