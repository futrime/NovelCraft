using System.IO.Compression;
using System.Text.Json;
using System.Text.RegularExpressions;
using NovelCraft.Server;
using NovelCraft.Server.Game;
using NovelCraft.Server.Recorder;
using NovelCraft.Server.Server;
using NovelCraft.Utilities.Logger;

ILogger logger = new Logger("Server");

logger.Info(@"Welcome to NovelCraft!

         _   _                _  _____            __ _   
        | \ | |              | |/ ____|          / _| |  
        |  \| | _____   _____| | |     _ __ __ _| |_| |_ 
        | . ` |/ _ \ \ / / _ \ | |    | '__/ _` |  _| __|
        | |\  | (_) \ V /  __/ | |____| | | (_| | | | |_ 
        |_| \_|\___/ \_/ \___|_|\_____|_|  \__,_|_|  \__|
        
      -------- A Minecraft-like game written in C# --------
");
logger.Info("Starting server");
logger.Info($"Version {typeof(Program).Assembly.GetName().Version?.ToString(3)}");


#region Load configurations
// Read the config file and deserialize it into a Config object.
string configJsonStr = File.ReadAllText("config.json");

var config = JsonSerializer.Deserialize<Config>(configJsonStr)!;

logger.Info($"Level name: {config.LevelName}");
#endregion


#region Initialize the recorder
Recorder? recorder = config.SaveRecord is true ? new(
  Path.Combine("worlds", config.LevelName, "records")) : null;
#endregion


#region Initialize the game
// Create a new game.
Game game = new(config);

// If the level does not exist but the packed level exists, unpack it.
if (!File.Exists($"worlds/{config.LevelName}/level.dat") && File.Exists($"worlds/{config.LevelName}.nclevel")) {
  logger.Info($"Extracting worlds/{config.LevelName}.nclevel");

  // Delete the target directory if it exists.
  if (Directory.Exists($"worlds/{config.LevelName}")) {
    Directory.Delete($"worlds/{config.LevelName}", true);
  }
  // Create the target directory.
  Directory.CreateDirectory($"worlds/{config.LevelName}");

  // Extract the .nclevel file.
  ZipFile.ExtractToDirectory($"worlds/{config.LevelName}.nclevel", $"worlds/{config.LevelName}");
}

// If both the level and the packed level exist, delete the packed level.
if (File.Exists($"worlds/{config.LevelName}/level.dat") && File.Exists($"worlds/{config.LevelName}.nclevel")) {
  File.Delete($"worlds/{config.LevelName}.nclevel");
}

// Remove level.dat.old and records.
if (File.Exists($"worlds/{config.LevelName}/level.dat.old")) {
  File.Delete($"worlds/{config.LevelName}/level.dat.old");
}
if (Directory.Exists($"worlds/{config.LevelName}/records/")) {
  Directory.Delete($"worlds/{config.LevelName}/records/", true);
}

if (!File.Exists($"worlds/{config.LevelName}/level.dat")) {
  throw new FileNotFoundException($"worlds/{config.LevelName}/level.dat does not exist.");
}

logger.Info($"Opening worlds/{config.LevelName}/level.dat");

if (config.SaveLevel) {
  // Mark the current level as level.dat.old.
  File.Copy($"worlds/{config.LevelName}/level.dat", $"worlds/{config.LevelName}/level.dat.old");
}

// Load the level.
using (ZipArchive levelDataZipFile = ZipFile.OpenRead($"worlds/{config.LevelName}/level.dat")) {
  game.LoadLevel(levelDataZipFile);
}

// Find all definition files under definitions/ recursively and load them.
// Note that registration must be done after the level is loaded.
foreach (string definitionFilePath in Directory.EnumerateFiles("definitions/", "*.json", SearchOption.AllDirectories)) {
  try {
    string definitionJsonString = File.ReadAllText(definitionFilePath);
    IDefinition definition = IDefinition.NewFromJsonStr(definitionJsonString);
    game.RegisterDefinition(definition);
  } catch (Exception e) {
    logger.Error($"Failed to load definition file {definitionFilePath}: {e.Message}");
    continue;
  }
}
#endregion


#region Initialize the server
// Load the whitelist.
Dictionary<string, AgentInfo> whitelist = JsonSerializer.Deserialize<Dictionary<string, AgentInfo>>(
  File.ReadAllText("whitelist.json"))!;
Dictionary<string, int> agentDict = new();
foreach (var kvp in whitelist) {
  int uniqueId = game.CreatePlayer();
  agentDict.Add(kvp.Key, uniqueId);
  whitelist[kvp.Key].UniqueId = uniqueId;
}

recorder?.Record(new NovelCraft.Server.Recorder.AfterAgentRegisterEventRecord() {
  Tick = 0,
  Data = new() {
    SpawnList = (from kvp in whitelist
                 select new NovelCraft.Server.Recorder.AfterAgentRegisterEventRecord.AgentType() {
                   UniqueId = kvp.Value.UniqueId,
                   Name = kvp.Value.Name,
                   Token = kvp.Key,
                 }
    ).ToList()
  }
});

Server server = new(config, agentDict);
#endregion


Binder binder = new(game, recorder, server);

server.Run();

if (config.WaitingTime > 0) {
  logger.Info($"Waiting for {config.WaitingTime} seconds...");
  Thread.Sleep((int)(config.WaitingTime * 1000));
}

game.Run();

// Show information.
logger.Info("NovelCraft is licensed under LGPLv3");
logger.Info("Star our repository on GitHub: https://github.com/NovelCraft/NovelCraft");
logger.Info("See documentation at https://novelcraft.games");
logger.Info("Input \"stop\" and enter to stop the server. Do not close the console directly.");

var userInputTaskTokenSource = new CancellationTokenSource();
Task userInputTask = Task.Run(() => {
  while (true) {
    if (userInputTaskTokenSource.IsCancellationRequested) {
      break;
    }

    string? input = Console.ReadLine();

    if (input is null || input == "") {
      continue;

    } else if (input.StartsWith("damage ") || input == "damage") {
      const string pattern = @"^damage (?<token>\S+) (?<amount>\d+)$";

      Match match = Regex.Match(input, pattern);
      if (!match.Success) {
        logger.Error($"Invalid command: {input}");
        continue;
      }

      string token = match.Groups["token"].Value;
      int amount = int.Parse(match.Groups["amount"].Value);

      try {
        int uniqueId = GetUniqueIdFromToken(token);
        var player = game.GetPlayer(uniqueId) ?? throw new Exception($"Player {token} does not exist");
        player.Damage(amount, new(NovelCraft.Server.Game.EntityDamageCause.KindType.None), game.CurrentTick);

        logger.Info($"Player {token} damaged by {amount}");

      } catch (Exception e) {
        logger.Error(e.Message);
        continue;
      }

    } else if (input.StartsWith("give ") || input == "give") {
      const string pattern = @"^give (?<token>\S+) (?<item>\S+)( (?<count>\d+))?$";

      Match match = Regex.Match(input, pattern);
      if (!match.Success) {
        logger.Error($"Invalid command: {input}");
        continue;
      }

      string token = match.Groups["token"].Value;
      string item = match.Groups["item"].Value;
      int count = match.Groups["count"].Success ? int.Parse(match.Groups["count"].Value) : 1;

      try {
        int uniqueId = GetUniqueIdFromToken(token);
        var player = game.GetPlayer(uniqueId) ?? throw new Exception($"Player {token} does not exist");
        int itemTypeId = game.ItemStackFactory.GetItemTypeIdFromIdentifier(item);
        var itemStack = game.ItemStackFactory.CreateItemStack(itemTypeId, count);
        player.GiveItem(itemStack);

        logger.Info($"{count} {item} given to {token}");

      } catch (Exception e) {
        logger.Error($"Failed to give item: {e.Message}");
      }

    } else if (input == "help" || input == "?") {
      logger.Info("Available commands:");
      logger.Info("  damage <token> <amount>: Damage <token> by <amount> hearts.");
      logger.Info("  give <token> <item> [count]: Give <count> <item> to <token>.");
      logger.Info("  help: Show this help message.");
      logger.Info("  stop: Stop the server.");

    } else if (input == "stop") {
      StopServer(config);
      Environment.Exit(0);

    } else {
      logger.Error($"Unknown command: {input}. Please check that the command exists and that you have permission to use it.");
    }
  }
});

// Wait for the game to max tick.
while (true) {
  if (config.MaxTick is not null && game.CurrentTick > config.MaxTick) {
    logger.Info("Max tick reached");
    userInputTaskTokenSource.Cancel();
    StopServer(config);
    Environment.Exit(0);
  }

  Thread.Sleep(100);
}


/// <summary>
/// Gets the unique ID of an agent from its token.
/// </summary>
/// <param name="token">The token of the agent.</param>
int GetUniqueIdFromToken(string token) {
  if (!whitelist.ContainsKey(token)) {
    throw new Exception($"Token {token} not found in whitelist.");
  }
  return whitelist[token].UniqueId;
}

/// <summary>
/// Stops the server.
/// </summary>
/// <param name="config">The server configuration.</param>
void StopServer(Config config) {
  logger.Info("Server stop requested.");
  logger.Info("Do not close the console directly. Otherwise, the level data may be corrupted.");

  // Stop the game.
  logger.Info("Stopping server...");
  game.Stop();

  // Save records.
  logger.Info("Saving records...");
  recorder?.Dispose();

  if (config.SaveLevel) {
    logger.Info("Saving level...");

    // Remove level.dat.
    File.Delete($"worlds/{config.LevelName}/level.dat");

    // Save the level.
    using (ZipArchive levelDataZipFile = ZipFile.Open($"worlds/{config.LevelName}/level.dat", ZipArchiveMode.Create)) {
      game.SaveLevel(levelDataZipFile);
    }
  }
}
