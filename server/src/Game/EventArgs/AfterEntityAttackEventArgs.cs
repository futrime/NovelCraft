namespace NovelCraft.Server.Game;

public class AfterEntityAttackEventArgs : EventArgs {
  public int CurrentTick { get; }
  public List<Game.AttackType> AttackList { get; }

  public AfterEntityAttackEventArgs(List<Game.AttackType> AttackList, int currentTick) {
    this.AttackList = AttackList;
    CurrentTick = currentTick;
  }
}