using System.Collections.Concurrent;
using System.Net.WebSockets;
using NovelCraft.Utilities.Logger;
using NovelCraft.Utilities.Messages;

namespace NovelCraft.Sdk;

internal class Client : IClient {
  public event EventHandler<IMessage>? AfterMessageReceiveEvent;


  private const int BufferSize = 134217728;
  private const int ByteCountPeriod = 1000;
  private const int MessageQueueCapacity = 100;
  private const int MessageSendPeriod = 10;

  /// <summary>
  /// Gets the bandwidth (in Mbps) between the client and the server.
  /// </summary>
  public decimal BandWidth { get; private set; } = 0;

  /// <summary>
  /// Gets the latency between the client and the server.
  /// </summary>
  public decimal Latency { get; private set; }

  private int _byteCountInThisPeriod = 0;
  private System.Timers.Timer _byteCountTimer;
  private ClientWebSocket _clientWebSocket;
  private ILogger _logger = new Logger("SDK.Client");
  private ConcurrentQueue<IMessage> _messageQueue = new();
  private byte[] _receiveBuffer = new byte[BufferSize];
  private Uri _uri;


  public Client(string host, int port) {
    _uri = new($"ws://{host}:{port}");
    _clientWebSocket = TryConnect();
    _logger.Info("Connected to server");

    _byteCountTimer = new System.Timers.Timer(ByteCountPeriod);
    _byteCountTimer.Elapsed += (sender, e) => {
      BandWidth = BandWidth * 0.5m + (decimal)_byteCountInThisPeriod * 8 / 1e3m / ByteCountPeriod * 0.5m;
      _byteCountInThisPeriod = 0;
    };
    _byteCountTimer.Start();

    Task.Run(() => {
      while (true) {
        ReceiveMessage();
      }
    });

    Task.Run(() => {
      while (true) {
        Thread.Sleep(MessageSendPeriod);
        if (_messageQueue.TryDequeue(out IMessage? message)) {
          _clientWebSocket.SendAsync(GetBuffer(message.JsonString), WebSocketMessageType.Text, true, CancellationToken.None);
        }
      }
    });
  }


  public void Send(IMessage message) {
    if (_messageQueue.Count >= MessageQueueCapacity) {
      _logger.Error("Message queue overflow");
      return;
    }

    _messageQueue.Enqueue(message);
  }

  /// <summary>Get buffer from a byte array</summary>
  /// <param name="arr">byte array</param>
  /// <returns> ArraySegment<byte> </returns>
  private ArraySegment<byte> GetBuffer(byte[] array) {
    return new ArraySegment<byte>(array);
  }

  /// <summary>Get buffer from a string</summary>
  /// <param name="str">string</param>
  /// <returns> ArraySegment<byte> </returns>
  private ArraySegment<byte> GetBuffer(string str) {
    return GetBuffer(System.Text.Encoding.UTF8.GetBytes(str));
  }

  private void ReceiveMessage() {
    int count = 0;

    try {
      WebSocketReceiveResult result = _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(_receiveBuffer), CancellationToken.None).Result;

      if (result.MessageType == WebSocketMessageType.Close) {
        throw new Exception("Server closed connection");
      }

      count = result.Count;

    } catch (Exception e) {
      _logger.Error($"Failed to receive message: {e.Message}");
      _clientWebSocket = TryConnect();
      return;
    }

    _byteCountInThisPeriod += count;

    if (count >= _receiveBuffer.Length) {
      _logger.Error("Buffer overflow");
      return;
    }

    try {
      IMessage message = Parser.Parse(
        System.Text.Encoding.UTF8.GetString(_receiveBuffer[..count]));

      AfterMessageReceiveEvent?.Invoke(this, message);
    } catch (Exception e) {
      _logger.Error($"Failed to parse message: {e.Message}: {System.Text.Encoding.UTF8.GetString(_receiveBuffer[..Math.Min(1024, count)])}...");
    }
  }

  private ClientWebSocket TryConnect() {
    ClientWebSocket clientWebSocket = new();
    _logger.Info($"Trying to connect to server at {_uri}...");

    while (true) {
      try {
        clientWebSocket.ConnectAsync(_uri, CancellationToken.None).Wait();
        break;
      } catch (Exception e) {
        _logger.Error($"Failed to connect to server: {e.Message}");
        clientWebSocket = new ClientWebSocket();
      }

      _logger.Info("Retrying...");
    }

    return clientWebSocket;
  }
}