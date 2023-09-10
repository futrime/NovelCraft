using System.Diagnostics;

namespace NovelCraft.Server.Game;

public partial class Entity {
  /// <summary>
  /// Initialize the components of the entity.
  /// </summary>
  public void InitializeComponents() {
    if (_definition.Components.Health is not null) {
      _health = _definition.Components.Health.Value;
      _lastDamageTick = SpawnTick;
    }
  }


  #region attack
  public decimal? AttackDamage {
    get {
      if (_definition.Components.Attack is null) {
        return null;
      }

      return _definition.Components.Attack.Damage;
    }
  }
  #endregion


  #region collision_box
  /// <summary>
  /// Gets the collision box of the block.
  /// </summary>
  public AABB? CollisionBox {
    get {
      // Check if the definition is loaded.
      if (_definition.Components.CollisionBox is null) {
        return null;
      }

      return new AABB(
        new Position<decimal>(
          Position.X - 0.5m * _definition.Components.CollisionBox.Width,
          Position.Y,
          Position.Z - 0.5m * _definition.Components.CollisionBox.Width
        ),
        new SizeType<decimal>(
          _definition.Components.CollisionBox.Width,
          _definition.Components.CollisionBox.Height,
          _definition.Components.CollisionBox.Width
        )
      );
    }
  }
  #endregion


  #region health
  /// <summary>
  /// The minimum ticks between damage.
  /// </summary>
  private const int MinDamageTick = 10;

  /// <summary>
  /// Gets the current health of the entity.
  /// </summary>
  public decimal? Health {
    get {
      if (_definition.Components.Health is null) {
        return null;
      }

      return _health;
    }
  }

  /// <summary>
  /// Gets a value indicating whether the entity is dead.
  /// </summary>
  public bool? IsDead {
    get {
      if (_definition.Components.Health is null) {
        return null;
      }

      return _health <= 0;
    }
  }

  /// <summary>
  /// Gets the maximum health of the entity.
  /// </summary>
  public decimal? MaxHealth {
    get {
      if (_definition.Components.Health is null) {
        return null;
      }

      return _definition.Components.Health.Max;
    }
  }

  /// <summary>
  /// Damages the entity.
  /// </summary>
  /// <param name="amount">The amount to damage.</param>
  /// <param name="cause">The cause of the damage.</param>
  /// <param name="tick">The tick when the damage occurred.</param>
  public void Damage(decimal amount, EntityDamageCause cause, int tick) {
    if (_definition.Components.Health is null) {
      throw new InvalidOperationException("The entity does not have health.");
    }

    if (tick - _lastDamageTick < MinDamageTick) {
      return;
    }

    // Check if the entity is dead.
    if (_health <= amount) {
      amount = _health;

      _health = 0;

      AfterHurtEvent?.Invoke(this, new AfterHurtEventArgs(this, amount, cause));
      AfterDespawnEvent?.Invoke(this, new AfterDespawnEventArgs(this));

    } else {
      _health -= amount;
      AfterHurtEvent?.Invoke(this, new AfterHurtEventArgs(this, amount, cause));
    }

    _lastDamageTick = tick;
  }

  /// <summary>
  /// Despawns the entity.
  /// </summary>
  public void Despawn() {
    if (_definition.Components.Health is null) {
      throw new InvalidOperationException("The entity does not have health.");
    }

    if (_health <= 0) {
      throw new InvalidOperationException("The entity is already dead.");
    }

    _health = 0;

    AfterDespawnEvent?.Invoke(this, new AfterDespawnEventArgs(this));
  }

  /// <summary>
  /// Heals the entity.
  /// </summary>
  /// <param name="amount">The amount to heal.</param>
  public void Heal(decimal amount) {
    if (_definition.Components.Health is null) {
      throw new InvalidOperationException("The entity does not have health.");
    }

    // Heal the entity.
    _health += amount;

    // Clamp the health.
    _health = Math.Clamp(_health, 0, _definition.Components.Health.Max);
  }

  /// <summary>
  /// Spawns the entity.
  /// </summary>
  public void Spawn() {
    if (_definition.Components.Health is null) {
      throw new InvalidOperationException("The entity does not have health.");
    }

    if (_health > 0) {
      throw new InvalidOperationException("The entity is already alive.");
    }

    _health = _definition.Components.Health.Value;

    AfterSpawnEvent?.Invoke(this, new AfterSpawnEventArgs(this));
  }

  private decimal _health;
  private int _lastDamageTick;
  #endregion


  #region movement
  public enum MovementDirection {
    Forward,
    Backward,
    Left,
    Right,
    Stopped
  }

  /// <summary>
  /// Gets the movement jump momentum of the entity.
  /// </summary>
  public decimal? MovementJumpMomentum {
    get {
      if (_definition.Components.MovementJump is null) {
        return null;
      }

      return _definition.Components.MovementJump.Value;
    }
  }

  /// <summary>
  /// Gets the movement power of the entity.
  /// </summary>
  public decimal? MovementPower {
    get {
      if (_definition.Components.Movement is null) {
        return null;
      }

      return _definition.Components.Movement.Value;
    }
  }

  private MovementDirection _movementDirection = MovementDirection.Stopped;

  /// <summary>
  /// Jumps.
  /// </summary>
  public void Jump() {
    if (_definition.Components.MovementJump is null) {
      throw new InvalidOperationException("The entity does not have movement jump.");
    }

    if (HasPhysics is not true) {
      throw new InvalidOperationException("The entity does not have physics.");
    }

    lock (this) {
      if (ContactingDirections![DirectionKind.Down] is not true) {
        return;
      }

      // Apply the impulse.
      decimal verticalVelocity = _definition.Components.MovementJump.Value / Mass!.Value;

      Velocity = new Velocity(Velocity.X, verticalVelocity, Velocity.Z);
    }
  }

  /// <summary>
  /// Sets the movement of the entity.
  /// </summary>
  /// <param name="direction">The direction to move.</param>
  public void SetMovement(MovementDirection direction) {
    if (_definition.Components.Movement is null) {
      throw new InvalidOperationException("The entity does not have movement.");
    }

    _movementDirection = direction;
  }
  #endregion


  #region physics

  private const decimal GravitationalAcceleration = -9.81m;

  /// <summary>
  /// Gets whether the entity is contacting with any blocks in the specified direction.
  /// </summary>
  /// <param name="direction">The direction to check.</param>
  /// <returns>A value indicating whether the entity is contacting with any blocks in the specified direction.</returns>
  public Dictionary<DirectionKind, bool>? ContactingDirections {
    get {
      if (HasPhysics is not true) {
        return null;
      }

      return _contactingDirections;
    }
  }

  /// <summary>
  /// Gets a value indicating whether the entity should be be included in the physics simulation.
  /// </summary>
  public bool HasPhysics => _definition.Components.Physics is not null;

  /// <summary>
  /// Gets the mass of the entity.
  /// </summary>
  public decimal? Mass => HasPhysics ? 1 : null;

  private Dictionary<DirectionKind, bool> _contactingDirections = new() {
    { DirectionKind.North, false },
    { DirectionKind.South, false },
    { DirectionKind.East, false },
    { DirectionKind.West, false },
    { DirectionKind.Up, false },
    { DirectionKind.Down, false }
  };

  /// <summary>
  /// Updates physics of the entity.
  /// </summary>
  /// <param name="level">The level</param>
  /// <param name="timeGap">The time gap</param>
  /// <param name="tick">The tick</param>
  public void UpdatePhysics(Level level, decimal timeInterval, int tick) {
    if (HasPhysics is not true) {
      throw new InvalidOperationException("The entity does not have physics.");
    }

    Stopwatch stopwatch = new();

    stopwatch.Start();

    // Update the contacting directions.
    if (CollisionBox is not null) {
      _contactingDirections = level.CheckContaction(CollisionBox);
    }

    var time1 = stopwatch.ElapsedTicks;
    stopwatch.Restart();

    Acceleration acceleration = new(GetFrictionalAcceleration(level) + GetGravitationalAcceleration() + GetMovementAcceleration());

    var time2 = stopwatch.ElapsedTicks;
    stopwatch.Restart();

    (Velocity nextVelocity, Position<decimal> nextPosition) = GetDisplacement(timeInterval, level, acceleration);

    var time3 = stopwatch.ElapsedTicks;
    stopwatch.Stop();

    // _logger.Debug($"Time1: {time1 / (decimal)Stopwatch.Frequency * 1000} ms | Time2: {time2 / (decimal)Stopwatch.Frequency * 1000} ms | Time3: {time3 / (decimal)Stopwatch.Frequency * 1000} ms");

    if (Health is not null) {
      decimal fallingDamage = GetFallingDamage(Velocity, nextVelocity);
      if (fallingDamage > 0) {
        Damage(fallingDamage, new EntityDamageCause(EntityDamageCause.KindType.Falling), tick);
      }
    }

    decimal displacementMagnitude = (Position - nextPosition).Length;
    decimal velocityDifferenceMagnitude = (Velocity - nextVelocity).Length;

    Velocity = nextVelocity;
    Position = nextPosition;

    // After this line, the entity is at the next position.

    if (Math.Abs(displacementMagnitude) > Epsilon || Math.Abs(velocityDifferenceMagnitude) > Epsilon) {
      AfterPositionChangeEvent?.Invoke(this, new AfterPositionChangeEventArgs(this));
    }
  }

  /// <summary>
  /// Gets next velocity and position of the entity.
  /// </summary>
  private (Velocity nextVelocity, Position<decimal> nextPosition) GetDisplacement(decimal timeInterval, Level level, Acceleration acceleration) {
    // If the entity is not colliding with any block, calculate the next position and velocity directly.
    if (CollisionBox is null) {
      return (new Velocity(Velocity + acceleration * timeInterval),
              new Position<decimal>(Position + Velocity * timeInterval + acceleration * timeInterval * timeInterval / 2));
    }

    // If the entity is stuck in a block, forbid it from moving.
    if (level.CheckCollisionWithAnyBlock(this.CollisionBox) is true) {
      return (new Velocity(0, 0, 0), Position);
    }

    Position<decimal> possibleNextPosition = new(Position + Velocity * timeInterval + acceleration * timeInterval * timeInterval / 2);
    Velocity possibleNextVelocity = new(Velocity + acceleration * timeInterval);

    // If the entity has stopped moving and is contacting with a block below it, forbid it from moving horizontally.
    if (_movementDirection == MovementDirection.Stopped && _contactingDirections[DirectionKind.Down] is true) {
      possibleNextPosition.X = Position.X;
      possibleNextPosition.Z = Position.Z;
      possibleNextVelocity.X = 0;
      possibleNextVelocity.Z = 0;
    }

    // The entity should not move in the direction of a block it is contacting.
    if (_contactingDirections[DirectionKind.North] is true) {
      possibleNextPosition.Z = Math.Min(possibleNextPosition.Z, Position.Z);
      possibleNextVelocity.Z = Math.Min(possibleNextVelocity.Z, 0);
    }
    if (_contactingDirections[DirectionKind.South] is true) {
      possibleNextPosition.Z = Math.Max(possibleNextPosition.Z, Position.Z);
      possibleNextVelocity.Z = Math.Max(possibleNextVelocity.Z, 0);
    }
    if (_contactingDirections[DirectionKind.East] is true) {
      possibleNextPosition.X = Math.Min(possibleNextPosition.X, Position.X);
      possibleNextVelocity.X = Math.Min(possibleNextVelocity.X, 0);
    }
    if (_contactingDirections[DirectionKind.West] is true) {
      possibleNextPosition.X = Math.Max(possibleNextPosition.X, Position.X);
      possibleNextVelocity.X = Math.Max(possibleNextVelocity.X, 0);
    }
    if (_contactingDirections[DirectionKind.Up] is true) {
      possibleNextPosition.Y = Math.Min(possibleNextPosition.Y, Position.Y);
      possibleNextVelocity.Y = Math.Min(possibleNextVelocity.Y, 0);
    }
    if (_contactingDirections[DirectionKind.Down] is true) {
      possibleNextPosition.Y = Math.Max(possibleNextPosition.Y, Position.Y);
      possibleNextVelocity.Y = Math.Max(possibleNextVelocity.Y, 0);
    }

    // Find the closest collision point or null if there is no collision.
    List<Position<decimal>?> possibleCollisionPoints = new() { null, null, null, null, null, null, null, null };

    for (int i = 0; i < this.CollisionBox.Vertices.Count; i++) {
      Position<decimal> vertex = this.CollisionBox.Vertices[i];
      Vector3<decimal> possibleDisplacement = possibleNextPosition - Position;
      Position<decimal> possibleNextVertex = new(vertex + possibleDisplacement);
      List<Position<decimal>> intersectionPointList = VectorMath.GetIntersectionPointList(vertex, possibleNextVertex); // Already sorted by distance.

      foreach (Position<decimal> point in intersectionPointList) {
        // Add Epsilon*diffPosition to the point to avoid floating point errors.
        AABB collisionBox = new AABB(new Position<decimal>(this.CollisionBox.Min + (point - vertex) + possibleDisplacement * Epsilon), this.CollisionBox.Size);

        if (level.CheckCollisionWithAnyBlock(collisionBox) is true) {
          possibleCollisionPoints[i] = point;
          break;
        }
      }
    }

    Position<decimal>? closestCollisionPoint = null;
    int? closestCollisionVertexIdx = null;
    decimal closestCollisionDistance = decimal.MaxValue;

    for (int i = 0; i < possibleCollisionPoints.Count; i++) {
      Position<decimal>? possibleCollisionPoint = possibleCollisionPoints[i];

      if (possibleCollisionPoint is null) {
        continue;
      }

      Position<decimal> vertex = this.CollisionBox.Vertices[i];

      if (closestCollisionPoint is null || (vertex - possibleCollisionPoint).Length < closestCollisionDistance) {
        closestCollisionPoint = possibleCollisionPoints[i];
        closestCollisionVertexIdx = i;
        closestCollisionDistance = (vertex - possibleCollisionPoint).Length;
      }
    }

    // If during the movement, the entity does not collide with any block, move it to the possible next position.
    if (closestCollisionPoint is null || closestCollisionVertexIdx is null) {
      return (possibleNextVelocity, possibleNextPosition);
    }
    
    possibleNextPosition = new(Position + (closestCollisionPoint - this.CollisionBox.Vertices[closestCollisionVertexIdx.Value]));

    /// If the entity will collide with a block without moving, set its velocity to zero.
    if (possibleNextPosition == Position) {
      possibleNextVelocity = new(0, 0, 0);
    }

    AABB possibleNextCollisionBox = new(new Position<decimal>(CollisionBox.Min + (possibleNextPosition - Position)), CollisionBox.Size);
    Dictionary<DirectionKind, bool> possibleNextContactingDirections = level.CheckContaction(possibleNextCollisionBox);

    if (possibleNextContactingDirections[DirectionKind.North] is true) {
      possibleNextVelocity.Z = Math.Min(possibleNextVelocity.Z, 0);
    }
    if (possibleNextContactingDirections[DirectionKind.South] is true) {
      possibleNextVelocity.Z = Math.Max(possibleNextVelocity.Z, 0);
    }
    if (possibleNextContactingDirections[DirectionKind.East] is true) {
      possibleNextVelocity.X = Math.Min(possibleNextVelocity.X, 0);
    }
    if (possibleNextContactingDirections[DirectionKind.West] is true) {
      possibleNextVelocity.X = Math.Max(possibleNextVelocity.X, 0);
    }
    if (possibleNextContactingDirections[DirectionKind.Up] is true) {
      possibleNextVelocity.Y = Math.Min(possibleNextVelocity.Y, 0);
    }
    if (possibleNextContactingDirections[DirectionKind.Down] is true) {
      possibleNextVelocity.Y = Math.Max(possibleNextVelocity.Y, 0);
    }

    if (level.CheckCollisionWithAnyBlock(possibleNextCollisionBox) is true) {
      _logger.Warning($"Entity {UniqueId} starts to be trapped in a block at {possibleNextPosition}.");
    }

    return (possibleNextVelocity, possibleNextPosition);
  }

  /// <summary>
  /// Gets the damage taken from falling.
  /// </summary>
  private decimal GetFallingDamage(Velocity currentVelocity, Velocity nextVelocity) {
    if (currentVelocity.Y > -Epsilon || Math.Abs(nextVelocity.Y) > Epsilon) {
      return 0;
    }

    // I = p = mv
    decimal verticalImpulseMagnitude = Math.Abs(nextVelocity.Y - currentVelocity.Y) * Mass!.Value;

    decimal fallDamage = (decimal)Math.Pow((double)(verticalImpulseMagnitude / Mass), 2) / (2 * Math.Abs(GravitationalAcceleration)) - 3;

    return Math.Max(0, fallDamage);
  }

  /// <summary>
  /// Gets the acceleration due to friction.
  /// </summary>
  private Acceleration GetFrictionalAcceleration(Level level) {
    // Friction is only applied when the entity is on the ground.
    if (_contactingDirections[DirectionKind.Down] is false) {
      return new Acceleration(0, 0, 0);
    }

    // Friction is only applied when the entity is on a block with a friction coefficient.
    Block blockUnderfoot = level.GetBlock(new Position<int> {
      X = (int)Position.X,
      Y = (int)(Position.Y - Epsilon),
      Z = (int)Position.Z
    });

    if (blockUnderfoot.FrictionCoefficient is null) {
      return new Acceleration(0, 0, 0);
    }

    // Friction is only applied when the entity is moving.
    Velocity horizontalVelocity = new(Velocity.X, 0, Velocity.Z);

    if (horizontalVelocity.Length < Epsilon) {
      return new Acceleration(0, 0, 0);
    }

    // a = μ * g
    decimal accelerationMagnitude = blockUnderfoot.FrictionCoefficient.Value * Math.Abs(GravitationalAcceleration);

    Acceleration acceleration = new(
      -accelerationMagnitude * horizontalVelocity.X / horizontalVelocity.Length,
      0,
      -accelerationMagnitude * horizontalVelocity.Z / horizontalVelocity.Length
    );

    return acceleration;
  }

  /// <summary>
  /// Gets the acceleration of gravity.
  /// </summary>
  private Acceleration GetGravitationalAcceleration() {
    return new Acceleration(0, GravitationalAcceleration, 0);
  }

  /// <summary>
  /// Gets the acceleration of the entity's movement.
  /// </summary>
  private Acceleration GetMovementAcceleration() {
    const decimal MaxMovementAcceleration = 10m;

    // The entity must be on the ground and have movement power to move.
    // Note that when the entity is not on the ground, the movement still has an effect on the entity's velocity.
    if (MovementPower is null) {
      return new Acceleration(0, 0, 0);
    }

    /// The entity must be contacting any block in any direction to move.
    // if (_contactingDirections.Values.All(contacting => contacting is false)) {
    //   return new Acceleration(0, 0, 0);
    // }

    // This magnitude must be greater than zero.
    decimal velocityMagnitude = Math.Max((decimal)Math.Sqrt((double)(Velocity.X * Velocity.X + Velocity.Z * Velocity.Z)), Epsilon);

    // F = P / v
    decimal forceMagnitude = MovementPower.Value / velocityMagnitude;

    // a = F / m
    decimal accelerationMagnitude = forceMagnitude / Mass!.Value;

    // Restrict the acceleration to the maximum.
    accelerationMagnitude = Math.Min(accelerationMagnitude, MaxMovementAcceleration);

    Acceleration acceleration = new(
      _movementDirection switch {
        MovementDirection.Forward => (decimal)Math.Sin((double)Orientation.Yaw * Math.PI / 180) * accelerationMagnitude,
        MovementDirection.Backward => -(decimal)Math.Sin((double)Orientation.Yaw * Math.PI / 180) * accelerationMagnitude,
        MovementDirection.Left => -(decimal)Math.Cos((double)Orientation.Yaw * Math.PI / 180) * accelerationMagnitude,
        MovementDirection.Right => (decimal)Math.Cos((double)Orientation.Yaw * Math.PI / 180) * accelerationMagnitude,
        _ => 0
      },
      0,
      _movementDirection switch {
        MovementDirection.Forward => (decimal)Math.Cos((double)Orientation.Yaw * Math.PI / 180) * accelerationMagnitude,
        MovementDirection.Backward => -(decimal)Math.Cos((double)Orientation.Yaw * Math.PI / 180) * accelerationMagnitude,
        MovementDirection.Left => (decimal)Math.Sin((double)Orientation.Yaw * Math.PI / 180) * accelerationMagnitude,
        MovementDirection.Right => -(decimal)Math.Sin((double)Orientation.Yaw * Math.PI / 180) * accelerationMagnitude,
        _ => 0
      }
    );

    return acceleration;
  }
  #endregion
}
