namespace NovelCraft.Server.Game;

/// <summary>
/// Game class is the main class of the game.
/// </summary>
public partial class Game {
  #region Static, const and readonly fields
  /// <summary>
  /// The default time gap between ticks.
  /// </summary>
  /// <remarks>
  /// This is not the TPS of the game. On the contrary, this is the time gap between ticks,
  /// which is the "real" duration between two ticks for physics simulation and other
  /// calculations.
  /// </remarks>
  private const decimal DefaultTimeGap = 0.05m;

  /// <summary>
  /// The interval of TPS check in seconds.
  /// </summary>
  private const decimal TpsCheckInterval = 5.0m;
  #endregion


  #region Fields and properties
  /// <summary>
  /// Gets the config of the game.
  /// </summary>
  public IConfig Config { get; }

  /// <summary>
  /// Gets the current tick of the game.
  /// </summary>
  /// <remarks>
  /// The first tick is 0.
  /// </remarks>
  public int CurrentTick { get; private set; } = 0;

  public decimal TicksPerSecond { get; private set; }

  private DateTime _lastTpsCheckTime = DateTime.Now;
  private DateTime? _lastTickTime = null; // In microseconds.
  private Task _tickTask;
  #endregion


  #region Constructors and finalizers
  /// <summary>
  /// Initializes a new instance of the <see cref="Game"/> class.
  /// </summary>
  public Game(IConfig config) {
    Config = config;
    TicksPerSecond = config.TicksPerSecond;

    _lootTableSource = new LootTableSource(ItemStackFactory);
    _recipeSource = new RecipeSource(ItemStackFactory);

    _tickTask = new Task(Tick);
  }
  #endregion


  #region Methods
  /// <summary>
  /// Runs the game.
  /// </summary>
  /// <remarks>
  /// Note that this is not used to resume the game after it has been paused.
  /// </remarks>
  public void Run() {
    if (_lastTickTime is not null) {
      throw new InvalidOperationException("The game is already running.");
    }

    _lastTickTime = DateTime.Now;

    _tickTask.Start();

    AfterGameStartEvent?.Invoke(this, new AfterGameRunEventArgs(this));
  }

  /// <summary>
  /// Stops the game.
  /// </summary>
  /// <remarks>
  /// Note that this is not used to pause the game.
  /// </remarks>
  public void Stop() {
    if (_lastTickTime is null) {
      _logger.Warning("The game is already stopped.");
    }

    lock (this) {
      _lastTickTime = null;
    }

    _tickTask.Wait();
  }

  /// <summary>
  /// Ticks the game. This method is called every tick to update the game.
  /// </summary>
  private void Tick() {
    while (true) {
      try {
        lock (this) {
          if (_lastTickTime is null) {
            break;
          }

          DateTime currentTime = DateTime.Now;

          if (currentTime - _lastTickTime < TimeSpan.FromSeconds((double)(1.0m / Config.TicksPerSecond))) {
            continue;
          }

          TicksPerSecond = 1.0m / (decimal)(currentTime - _lastTickTime.Value).TotalSeconds;
          _lastTickTime = currentTime;

          // Check TPS.
          if (DateTime.Now - _lastTpsCheckTime > TimeSpan.FromSeconds((double)TpsCheckInterval)) {
            _lastTpsCheckTime = DateTime.Now;
            if (TicksPerSecond < Config.TicksPerSecond * 0.9m) {
              _logger.Warning($"Insufficient simulation rate: {TicksPerSecond:0.00} tps < {Config.TicksPerSecond} tps");
            }
          }

          UpdateEntities();
          UpdateBlocks();

          AfterGameTickEvent?.Invoke(this, new AfterGameTickEventArgs(this, CurrentTick));

          // Accumulate the current tick at the end of the tick.
          CurrentTick++;
        }
        
      } catch (Exception e) {
        _logger.Error($"An exception occurred while ticking the game: {e}");
      }
    }
  }
  #endregion
}
