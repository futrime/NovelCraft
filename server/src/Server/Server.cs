using System.IO.Compression;
using Fleck;
using NovelCraft.Utilities.Logger;
using NovelCraft.Utilities.Messages;

namespace NovelCraft.Server.Server;

/// <summary>
/// Server class is the main class of the server.
/// </summary>
public class Server {
  #region Nested classes, enums, delegates and events
  public event EventHandler<AfterReceiveMessageEventArgs>? AfterReceiveMessageEvent;
  #endregion


  #region Static, const and readonly fields
  #endregion


  #region Fields and properties
  private Config _config { get; }
  private ILogger _logger = new Logger("Server.Server");
  private Dictionary<string, IWebSocketConnection> _socketDict = new();
  private WebSocketServer _webSocketServer;
  private Dictionary<string, int> _agentDict = new(); // Token -> UniqueId
  #endregion


  #region Constructors and finalizers
  /// <summary>
  /// Initializes a new instance of the <see cref="Server"/> class.
  /// </summary>
  /// <param name="config">The configuration.</param>
  /// <param name="agentDict">The agent dictionary.</param>
  public Server(Config config, Dictionary<string, int> agentDict) {
    _config = config;
    _agentDict = agentDict;

    // Set Fleck logging
    FleckLog.LogAction = (level, message, ex) => {
      switch (level) {
        case LogLevel.Debug:
          // _logger.Debug(message);
          break;

        case LogLevel.Info:
          _logger.Info(message);
          break;

        case LogLevel.Warn:
          _logger.Warning(message);
          break;

        case LogLevel.Error:
          _logger.Error(message);
          break;
      }
    };

    _webSocketServer = new WebSocketServer($"ws://0.0.0.0:{_config.ServerPort}");

    _webSocketServer.RestartAfterListenError = true;
  }
  #endregion


  #region Methods
  /// <summary>
  /// Broadcasts a message to all agents.
  /// </summary>
  /// <param name="message">The message to broadcast.</param>
  public void Broadcast(IMessage message) {
    foreach (var kvp in _agentDict) {
      if (_socketDict.ContainsKey(kvp.Key)) {
        Send(kvp.Value, message);
      }
    }
  }

  /// <summary>
  /// Runs the server.
  /// </summary>
  public void Run() {
    _webSocketServer.Start(socket => {
      socket.OnOpen = () => {
        _logger.Info($"Client {socket.ConnectionInfo.ClientIpAddress}:{socket.ConnectionInfo.ClientPort} connected");
      };

      socket.OnClose = () => {
        RemoveSocket(socket);
      };

      socket.OnMessage = text => {
        try {
          IMessage message = Parser.Parse(text);

          if (message is not IClientMessage clientMessage) {
            throw new InvalidOperationException("Message is not a client message");
          }

          string token = ((IClientMessage)message).Token;

          if (!_agentDict.ContainsKey(token)) {
            ErrorMessage errorMessage = new() {
              Message = "Token is not registered",
              Code = 100,
            };
            socket.Send(errorMessage.JsonString);
            throw new InvalidOperationException("Token is not registered");
          }

          if (_socketDict.ContainsKey(token) && _socketDict[token] != socket) {
            RemoveSocket(_socketDict[token]);
          }

          if (!_socketDict.ContainsKey(token)) {
            _socketDict[token] = socket;
          }

          AfterReceiveMessageEvent?.Invoke(this, new AfterReceiveMessageEventArgs(_agentDict[token], message));

          if (message is ClientPingMessage msg) {
            int uniqueId = _agentDict[token];
            Send(uniqueId, new ServerPongMessage() {
              SentTime = msg.SentTime,
            });
          }
        } catch (Exception e) {
          _logger.Error($"Error occurs when receiving a message: {e.Message}");
          socket.Close();
        }
      };

      socket.OnError = exception => {
        _logger.Error($"Client {socket.ConnectionInfo.ClientIpAddress}:{socket.ConnectionInfo.ClientPort} error: {exception.Message}");
        socket.Close();
      };
    });

    _logger.Info("Server started");
  }

  /// <summary>
  /// Registers an agent.
  /// </summary>
  public void RegisterAgent(string token, int uniqueId) {
    if (_agentDict.ContainsKey(token)) {
      throw new InvalidOperationException("Token is already registered");
    }

    _agentDict.Add(token, uniqueId);
  }

  public void Send(int uniqueId, IMessage message) {
    try {
      string? token = null;
      foreach (var kvp in _agentDict) {
        if (kvp.Value == uniqueId) {
          token = kvp.Key;
          break;
        }
      }

      if (token is null) {
        throw new InvalidOperationException("UniqueId is not registered");
      }

      if (!_socketDict.ContainsKey(token)) {
        _logger.Warning($"Agent {token} is not connected but attempted to send a message");
        return;
      }

      string jsonStr = message.JsonString;
      // byte[] compressedBytes = CompressString(Encoding.UTF8.GetBytes(jsonStr));
      var socket = _socketDict[token];
      socket.Send(jsonStr);
      
    } catch (Exception e) {
      _logger.Error($"Error occurs when sending a message: {e.Message}");
    }
  }

  private void RemoveSocket(IWebSocketConnection socket) {
    // Remove the socket in _socketDict
    List<string> keysToRemove = new();
    foreach (var kvp in _socketDict) {
      if (kvp.Value == socket) {
        keysToRemove.Add(kvp.Key);
      }
    }

    foreach (string key in keysToRemove) {
      _socketDict.Remove(key);
    }

    _logger.Info($"Client {socket.ConnectionInfo.ClientIpAddress}:{socket.ConnectionInfo.ClientPort} disconnected");
  }

  static private byte[] CompressString(byte[] bytes) {
    MemoryStream memory = new();
    using (GZipStream gzip = new(memory, CompressionMode.Compress, true)) {
      gzip.Write(bytes, 0, bytes.Length);
    }

    memory.Position = 0;

    byte[] compressed = new byte[memory.Length];
    memory.Read(compressed, 0, compressed.Length);

    byte[] gzBuffer = new byte[compressed.Length + 4];
    Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
    Buffer.BlockCopy(BitConverter.GetBytes(bytes.Length), 0, gzBuffer, 0, 4);
    return gzBuffer;
  }

  static private byte[] DecompressString(byte[] gzBuffer) {
    using (MemoryStream memory = new()) {
      int msgLength = BitConverter.ToInt32(gzBuffer, 0);
      memory.Write(gzBuffer, 4, gzBuffer.Length - 4);

      byte[] buffer = new byte[msgLength];

      memory.Position = 0;
      using (GZipStream gzip = new(memory, CompressionMode.Decompress)) {
        gzip.Read(buffer, 0, buffer.Length);
      }

      return buffer;
    }
  }
  #endregion
}
