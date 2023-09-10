using NovelCraft.Utilities.Messages;

namespace NovelCraft.Server.Server;

public class AfterReceiveMessageEventArgs : EventArgs {
  public int UniqueId { get; }
  public IMessage Message { get; }

  public AfterReceiveMessageEventArgs(int uniqueId, IMessage message) {
    UniqueId = uniqueId;
    Message = message;
  }
}