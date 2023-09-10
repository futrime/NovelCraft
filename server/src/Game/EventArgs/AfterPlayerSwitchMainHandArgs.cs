namespace NovelCraft.Server.Game;

public class AfterPlayerSwitchMainHandArgs : EventArgs {
  public List<Game.PlayerSWitchMainHandType> ChangeList { get; }
  public int CurrentTick { get; }

  public AfterPlayerSwitchMainHandArgs(List<Game.PlayerSWitchMainHandType> changeList, int currentTick) {
    ChangeList = changeList;
    CurrentTick = currentTick;
  }
}