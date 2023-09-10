namespace NovelCraft.Sdk
{

    /// <summary>
    /// An agent is a player that can be controlled by an AI.
    /// </summary>
    public interface IAgent : IEntity
    {
        public enum InteractionKind { Click, HoldStart, HoldEnd }
        public enum UseKind { Click }
        public enum MovementKind { Forward, Backward, Left, Right }

        /// <summary>
        /// Gets the health of the olayer.
        /// </summary>
        public decimal Health { get; }

        /// <summary>
        /// Gets the inventory of the agent.
        /// </summary>
        public IInventory Inventory { get; }

        /// <summary>
        /// Gets or sets the movement of the agent.
        /// </summary>
        /// <remarks>
        /// Null means stopped.
        /// </remarks>
        public MovementKind? Movement { get; set; }

        /// <summary>
        /// Gets the token of the agent.
        /// </summary>
        public string Token { get; }

        /// <summary>
        /// Performs an attack.
        /// </summary>
        /// <param name="kind">The kind of attack to perform.</param>
        public void Attack(InteractionKind kind);

        /// <summary>
        /// Performs a jump.
        /// </summary>
        public void Jump();

        /// <summary>
        /// Makes the agent look at a position.
        /// </summary>
        /// <param name="position">The position to look at.</param>
        public void LookAt(IPosition<decimal> position);

        /// <summary>
        /// Sets the movement of the agent.
        /// </summary>
        /// <param name="kind">The kind of movement to perform. Null means stopped.</param>
        public void SetMovement(MovementKind? kind);

        /// <summary>
        /// Performs a use action.
        /// </summary>
        /// <param name="kind">The kind of use action to perform.</param>
        public void Use(UseKind kind);
    }

}