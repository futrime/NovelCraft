namespace NovelCraft.Sdk;

/// <summary>
/// Represents common interfaces for all communication clients.
/// </summary>
public interface IClient {
  /// <summary>
  /// Occurs when a message is received from the server.
  /// </summary>
  public event EventHandler<NovelCraft.Utilities.Messages.IMessage>? AfterMessageReceiveEvent;


  public decimal BandWidth { get; }


  /// <summary>
  /// Sends a message to the server.
  /// </summary>
  /// <param name="message">The message to be sent.</param>
  public void Send(NovelCraft.Utilities.Messages.IMessage message);
}