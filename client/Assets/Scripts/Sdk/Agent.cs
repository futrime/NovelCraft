using NovelCraft.Utilities.Messages;
using System;

namespace NovelCraft.Sdk
{

    internal class Agent : Entity, IAgent
    {
        public IInventory Inventory => _inventory;

        public decimal Health { get; set; } = 0;

        public IAgent.MovementKind? Movement
        {
            get => _movement;
            set
            {
                if (value is null)
                {
                    _movement = null;
                    return;
                }

                ClientPerformMoveMessage message = new()
                {
                    Token = Token,
                    DirectionType = value switch
                    {
                        IAgent.MovementKind.Forward => ClientPerformMoveMessage.Direction.Forward,
                        IAgent.MovementKind.Backward => ClientPerformMoveMessage.Direction.Backward,
                        IAgent.MovementKind.Left => ClientPerformMoveMessage.Direction.Left,
                        IAgent.MovementKind.Right => ClientPerformMoveMessage.Direction.Right,
                        null => ClientPerformMoveMessage.Direction.Stop,
                        _ => throw new NotImplementedException()
                    }
                };

                Sdk.Client?.Send(message);
                _movement = value;
            }
        }

        public string Token { get; }

        internal Inventory _inventory = new();

        private IAgent.MovementKind? _movement = null;


        public Agent(string token, int uniqueId, IPosition<decimal> position,
          IOrientation orientation) : base(uniqueId, 0, position, orientation)
        {
            Token = token;
        }


        public void Attack(IAgent.InteractionKind kind)
        {
            Sdk.Client?.Send(new ClientPerformAttackMessage()
            {
                Token = Token,
                AttackKind = kind switch
                {
                    IAgent.InteractionKind.Click => ClientPerformAttackMessage.AttackType.AttackClick,
                    IAgent.InteractionKind.HoldStart => ClientPerformAttackMessage.AttackType.HoldStart,
                    IAgent.InteractionKind.HoldEnd => ClientPerformAttackMessage.AttackType.HoldEnd,
                    _ => throw new NotImplementedException()
                }
            });
        }

        public void Jump()
        {
            Sdk.Client?.Send(new ClientPerformJumpMessage()
            {
                Token = Token
            });
        }

        public void LookAt(IPosition<decimal> position)
        {
            Sdk.Client?.Send(new ClientPerformLookAtMessage()
            {
                Token = Token,
                LookAtPosition = new()
                {
                    X = position.X,
                    Y = position.Y,
                    Z = position.Z
                }
            });
        }

        public void SetMovement(IAgent.MovementKind? kind)
        {
            Sdk.Client?.Send(new ClientPerformMoveMessage()
            {
                Token = Token,
                DirectionType = kind switch
                {
                    IAgent.MovementKind.Forward => ClientPerformMoveMessage.Direction.Forward,
                    IAgent.MovementKind.Backward => ClientPerformMoveMessage.Direction.Backward,
                    IAgent.MovementKind.Left => ClientPerformMoveMessage.Direction.Left,
                    IAgent.MovementKind.Right => ClientPerformMoveMessage.Direction.Right,
                    null => ClientPerformMoveMessage.Direction.Stop,
                    _ => throw new NotImplementedException()
                }
            });
        }

        public void Use(IAgent.UseKind kind)
        {
            Sdk.Client?.Send(new ClientPerformUseMessage()
            {
                Token = Token,
                UseType = kind switch
                {
                    IAgent.UseKind.Click => ClientPerformUseMessage.UseKind.UseClick,
                    _ => throw new NotImplementedException()
                }
            });
        }
    }

}