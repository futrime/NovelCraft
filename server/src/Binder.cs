using NovelCraft.Utilities.Logger;

namespace NovelCraft.Server;

/// <summary>
/// Binder binds the game, the recorder and the server.
/// </summary>
public partial class Binder {
  private NovelCraft.Server.Game.Game _game;
  private Logger _logger = new("Server.Binder");
  private NovelCraft.Server.Recorder.Recorder? _recorder;
  private NovelCraft.Server.Server.Server _server;

  public Binder(NovelCraft.Server.Game.Game game,
    NovelCraft.Server.Recorder.Recorder? recorder,
    NovelCraft.Server.Server.Server server) {
    _game = game;
    _recorder = recorder;
    _server = server;

    SubscribeToGame();
    SubscribeToServer();
  }
}