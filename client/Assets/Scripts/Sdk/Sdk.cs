using System;
using System.Collections.Generic;
using System.Linq;
using NovelCraft.Utilities.Logger;
using NovelCraft.Utilities.Messages;

namespace NovelCraft.Sdk
{

    /// <summary>
    /// The SDK.
    /// </summary>
    public static partial class Sdk
    {
        private const int GetInfoInterval = 1000;
        private const int PingInterval = 1000;


        /// <summary>
        /// Gets the agent representing the player controlled by the user.
        /// </summary>
        public static IAgent? Agent => _agent;

        /// <summary>
        /// Gets the block collection.
        /// </summary>
        public static IBlockSource? Blocks => _blockSource;

        /// <summary>
        /// Gets the client for sending and receiving messages directly to or from the server.
        /// </summary>
        public static IClient? Client => _client;

        /// <summary>
        /// Gets the list of all entities in the world.
        /// </summary>
        public static IEntitySource? Entities => _entitySource;

        /// <summary>
        /// Gets the logger.
        /// </summary>
        public static ILogger Logger => _userLogger;

        /// <summary>
        /// Gets the latency between the client and the server.
        /// </summary>
        public static TimeSpan? Latency => _latency;

        /// <summary>
        /// Gets current tick.
        /// </summary>
        public static int? Tick
        {
            get
            {
                if (_lastTickInfo is null)
                {
                    return null;
                }

                if (TicksPerSecond is null)
                {
                    return _lastTickInfo.Value.LastTick;
                }

                var timeSinceLastTick = DateTime.Now - _lastTickInfo.Value.LastTickTime;
                var ticksSinceLastTick = (int)((decimal)timeSinceLastTick.TotalSeconds * TicksPerSecond);

                return _lastTickInfo.Value.LastTick + ticksSinceLastTick;
            }
        }

        /// <summary>
        /// Gets the number of ticks per second.
        /// </summary>
        public static decimal? TicksPerSecond { get; private set; } = null;


        internal static Agent? _agent = null;
        internal static BlockSource? _blockSource = null;
        internal static Client? _client = null;
        internal static EntitySource? _entitySource = null;
        internal static TimeSpan? _latency = null;
        internal static ILogger _sdkLogger { get; } = new Logger("SDK");
        internal static ILogger _userLogger { get; } = new Logger("User");

        private static (int LastTick, DateTime LastTickTime)? _lastTickInfo = null;
        private static System.Timers.Timer _getInfoTimer = new(GetInfoInterval);
        private static System.Timers.Timer _pingTimer = new(PingInterval);
        private static string? _token = null;

        public static void CloseTimers()
        {
            _pingTimer.Close();
            _getInfoTimer.Close();
        }
        /// <summary>
        /// Initializes the SDK.
        /// </summary>
        /// <param name="config">The configuration of the SDK.</param>
        public static void Initialize(string token, string host, int port)
        {
            try
            {
                _sdkLogger.Info("Initializing SDK...");

                _token = token;

                // Initialize the client
                _client = new Client(host, port);
                _client.AfterMessageReceiveEvent += OnAfterMessageReceiveEvent;

                // Manually call the tick event to get information right away.
                OnTick();

                // Initialize the timers.
                _getInfoTimer.Elapsed += (sender, e) => OnTick();
                _getInfoTimer.Start();

                _pingTimer.Elapsed += (sender, e) =>
                {
                    Client?.Send(new ClientPingMessage()
                    {
                        Token = _token!,
                        SentTime = (decimal)DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond
                    });
                };
                _pingTimer.Start();

            }
            catch (Exception e)
            {
                throw new Exception($"Failed to initialize SDK: {e.Message}", e);
            }
        }

        private static void OnAfterMessageReceiveEvent(object? sender, IMessage message)
        {
            switch (message)
            {
                case ServerPongMessage msg:
                    _latency = TimeSpan.FromMilliseconds((double)(DateTime.Now.Ticks - (long)(msg.SentTime * TimeSpan.TicksPerMillisecond)) / TimeSpan.TicksPerMillisecond);
                    break;


                case ErrorMessage msg:
                    _sdkLogger.Error($"The server returned an error: {msg.Message} ({msg.Code})");
                    break;


                case ServerGetBlocksAndEntitiesMessage msg:
                    // Update the block source.
                    if (_blockSource is null)
                    {
                        _blockSource = new BlockSource();
                    }

                    _blockSource.Clear();
                    foreach (var section in msg.Sections)
                    {
                        _blockSource.AddSection(
                          new Section(
                            new Position<int>(section.Position.X, section.Position.Y, section.Position.Z),
                            section.Blocks
                          )
                        );
                    }

                    // Update the entity source.
                    if (_entitySource is null)
                    {
                        _entitySource = new EntitySource();
                    }

                    _entitySource.Clear();
                    foreach (var entity in msg.Entities)
                    {
                        _entitySource[entity.UniqueId] = new Entity(entity.UniqueId, entity.TypeId,
                          new Position<decimal>(entity.Position.X, entity.Position.Y, entity.Position.Z),
                          new Orientation(entity.Orientation.Yaw, entity.Orientation.Pitch));

                        if (_agent is not null && entity.UniqueId == _agent.UniqueId)
                        {
                            _agent.Position = new Position<decimal>(entity.Position.X, entity.Position.Y, entity.Position.Z);
                            _agent.Orientation = new Orientation(entity.Orientation.Yaw, entity.Orientation.Pitch);
                        }
                    }
                    break;


                case ServerGetPlayerInfoMessage msg:
                    if (_token is null)
                    {
                        throw new InvalidOperationException("The token is null.");
                    }

                    if (_agent is null)
                    {
                        _agent = new Agent(_token, msg.UniqueId,
                          new Position<decimal>()
                          {
                              X = msg.Position.X,
                              Y = msg.Position.Y,
                              Z = msg.Position.Z
                          },
                          new Orientation()
                          {
                              Yaw = msg.Orientation.Yaw,
                              Pitch = msg.Orientation.Pitch
                          });
                    }

                    if (msg.Inventory.Count != _agent.Inventory.Size)
                    {
                        throw new InvalidOperationException("The inventory size of the agent is not equal to the inventory size of the server.");
                    }

                    if (msg.UniqueId != _agent.UniqueId)
                    {
                        throw new InvalidOperationException("The unique ID of the agent is not equal to the unique ID of the server.");
                    }

                    _agent.Health = msg.Health;
                    _agent.Orientation = new Orientation(msg.Orientation.Yaw, msg.Orientation.Pitch);
                    _agent.Position = new Position<decimal>(msg.Position.X, msg.Position.Y, msg.Position.Z);
                    _agent._inventory.MainHandSlot = msg.MainHand;
                    for (int i = 0; i < msg.Inventory.Count; i++)
                    {
                        var itemInfo = msg.Inventory[i];
                        _agent._inventory[i] = (itemInfo is null) ? null : new ItemStack(itemInfo.TypeId, itemInfo.Count);
                    }
                    break;


                case ServerGetTickMessage serverGetTickMessage:
                    _lastTickInfo = (serverGetTickMessage.Tick, DateTime.Now);
                    TicksPerSecond = serverGetTickMessage.TicksPerSecond;
                    break;


                case ServerAfterBlockChangeMessage msg:
                    // To be tested
                    if (Blocks is not null)
                    {
                        foreach (var blockChangeInfo in msg.ChangeList)
                        {
                            Position<int> blockPosition = new Position<int>(blockChangeInfo.Position.X, blockChangeInfo.Position.Y, blockChangeInfo.Position.Z);
                            ((BlockSource)Blocks)[blockPosition] = new Block(blockChangeInfo.BlockTypeId, blockPosition);
                        }
                    }
                    break;


                case ServerAfterEntityCreateMessage msg:
                    if (Entities is not null)
                    {
                        foreach (var entityCreateInfo in msg.CreationList)
                        {
                            EntitySource entitySource = (EntitySource)Entities;

                            // Have checked if the key exists in the function "AddEntity"
                            entitySource.AddEntity(new Entity(entityCreateInfo.UniqueId, entityCreateInfo.EntityTypeId,
                            new Position<decimal>(entityCreateInfo.Position.X, entityCreateInfo.Position.Y, entityCreateInfo.Position.Z),
                            new Orientation(entityCreateInfo.Orientation.Yaw, entityCreateInfo.Orientation.Pitch)));
                        }
                    }
                    break;


                case ServerAfterEntityRemoveMessage msg:
                    if (Entities is not null)
                    {
                        foreach (int removalId in msg.RemovalIdList)
                        {
                            // Have checked if the key exists in the function "RemoveEntity"
                            ((EntitySource)Entities).RemoveEntity(removalId);
                        }
                    }
                    break;


                case ServerAfterEntityOrientationChangeMessage msg:
                    if (Entities is not null)
                    {
                        foreach (var changeInfo in msg.ChangeList)
                        {
                            var entity = Entities[changeInfo.UniqueId];
                            if (entity is not null)
                            {
                                ((EntitySource)Entities)[changeInfo.UniqueId] = new Entity(entity.UniqueId, entity.TypeId, entity.Position,
                                new Orientation(changeInfo.Orientation.Yaw, changeInfo.Orientation.Pitch));
                            }

                            if (_agent is not null && changeInfo.UniqueId == _agent.UniqueId)
                            {
                                _agent.Orientation = new Orientation(changeInfo.Orientation.Yaw, changeInfo.Orientation.Pitch);
                            }
                        }
                    }
                    break;


                case ServerAfterEntityPositionChangeMessage msg:
                    if (Entities is not null)
                    {
                        foreach (var changeInfo in msg.ChangeList)
                        {
                            var entity = Entities[changeInfo.UniqueId];
                            if (entity is not null)
                            {
                                ((EntitySource)Entities)[changeInfo.UniqueId] = new Entity(entity.UniqueId, entity.TypeId,
                                new Position<decimal>(changeInfo.Position.X, changeInfo.Position.Y, changeInfo.Position.Z),
                                entity.Orientation, new Position<decimal>(changeInfo.Velocity.X, changeInfo.Velocity.Y, changeInfo.Velocity.Z));
                            }

                            if (_agent is not null && changeInfo.UniqueId == _agent.UniqueId)
                            {
                                _agent.Position = new Position<decimal>(changeInfo.Position.X, changeInfo.Position.Y, changeInfo.Position.Z);
                                _agent.Velocity = new Position<decimal>(changeInfo.Velocity.X, changeInfo.Velocity.Y, changeInfo.Velocity.Z);
                            }
                        }
                    }
                    break;


                case ServerAfterPlayerInventoryChangeMessage msg:
                    // TO DO: Implement this case
                    break;
            }
        }

        private static void OnTick()
        {
            string token = _token ?? throw new InvalidOperationException("The SDK is not initialized.");

            Client?.Send(new ClientGetTickMessage()
            {
                Token = token
            });

            if (Agent is not null)
            {
                List<NovelCraft.Utilities.Messages.Position<int>> requestPositionList = (
                  from x in Enumerable.Range(-1, 3)
                  from y in Enumerable.Range(-1, 3)
                  from z in Enumerable.Range(-1, 3)
                  select new NovelCraft.Utilities.Messages.Position<int>()
                  {
                      X = (int)(Agent.Position.X + x * 16),
                      Y = (int)(Agent.Position.Y + y * 16),
                      Z = (int)(Agent.Position.Z + z * 16)
                  }
                ).ToList();

                Client?.Send(new ClientGetBlocksAndEntitiesMessage()
                {
                    Token = token,
                    RequestSectionList = requestPositionList
                });
            }

            Client?.Send(new ClientGetPlayerInfoMessage()
            {
                Token = token
            });
        }
    }

}