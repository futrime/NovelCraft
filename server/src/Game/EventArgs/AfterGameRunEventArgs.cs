namespace NovelCraft.Server.Game;

public class AfterGameRunEventArgs : EventArgs {
  public Game Game { get; }

  public AfterGameRunEventArgs(Game game) {
    Game = game;
  }
}