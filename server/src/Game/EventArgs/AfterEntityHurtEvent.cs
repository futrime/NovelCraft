namespace NovelCraft.Server.Game;

public class AfterEntityHurtEventArgs : EventArgs {
  public int CurrentTick { get; }
  public List<Game.HurtType> HurtList { get; }

  public AfterEntityHurtEventArgs(List<Game.HurtType> HurtList, int currentTick) {
    this.HurtList = HurtList;
    CurrentTick = currentTick;
  }
}