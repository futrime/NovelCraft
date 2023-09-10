namespace NovelCraft.Server.Game;

public class AfterGameTickEventArgs : EventArgs {
  public int CurrentTick { get; }
  public Game Game { get; }

  public AfterGameTickEventArgs(Game game, int currentTick) {
    Game = game;
    CurrentTick = currentTick;
  }
}