namespace NovelCraft.Server.Game;

/// <summary>
/// Entity
/// </summary>
public partial class Entity {
  #region Nested classes, enums, delegates and events
  public enum InteractionKind { Click, HoldStart, HoldEnd }
  #endregion


  #region Static, const and readonly fields
  private const decimal Epsilon = 1e-6m; // When two decimal numbers are less than this value, they are considered equal.
  #endregion


  #region Fields and properties
  /// <summary>
  /// Gets the absolute position of the block the entity is in.
  /// </summary>
  public Position<int> BlockPosition => new((int)Math.Floor(Position.X), (int)Math.Floor(Position.Y), (int)Math.Floor(Position.Z));

  /// <summary>
  /// Gets the absolute position of the eye of the entity.
  /// </summary>
  public Position<decimal> EyePosition {
    get {
      return new Position<decimal>(
        Position.X,
        Position.Y + _definition.Components.CollisionBox?.EyeHeight ?? 0,
        Position.Z
      );
    }
  }

  public string Identifier => _definition.Description.Identifier;

  public bool IsAttackingHold { get; private set; } = false;

  public Orientation Orientation {
    get => _orientation;
    set {
      _orientation = value;
      AfterOrientationChangeEvent?.Invoke(this, new AfterOrientationChangeEventArgs(this));
    }
  }

  public Position<decimal> Position {
    get; set;
  }

  public int SpawnTick { get; }

  public Position<decimal>? TeleportDestination { get; set; }

  public int TypeId => _definition.Description.TypeId;

  public int UniqueId { get; set; }

  public Velocity Velocity { get; set; } = new(0, 0, 0);

  private EntityDefinition _definition;
  private NovelCraft.Utilities.Logger.Logger _logger = new("Server.Game.Entity");
  private Orientation _orientation = new(0, 0);
  #endregion


  #region Constructors and finalizers
  public Entity(EntityDefinition definition, int uniqueId, Position<decimal> position, int tickCreated) {
    _definition = definition;
    UniqueId = uniqueId;
    Position = position;
    SpawnTick = tickCreated;

    InitializeComponents();
  }
  #endregion


  #region Methods
  /// <summary>
  /// Attacks.
  /// </summary>
  /// <param name="kind">The kind of attack</param>
  /// <param name="tick">The tick</param>
  public void Attack(InteractionKind kind, int tick) {
    switch (kind) {
      case InteractionKind.Click:
        if (IsAttackingHold) { // If the entity is holding the attack button, end the hold.
          Attack(InteractionKind.HoldEnd, tick);
        }

        TryAttackEvent?.Invoke(this, new TryAttackEventArgs(this, InteractionKind.Click, tick));
        break;

      case InteractionKind.HoldStart:
        if (IsAttackingHold) { // If the entity is holding the attack button, end last hold.
          Attack(InteractionKind.HoldEnd, tick);
        }

        IsAttackingHold = true;
        TryAttackEvent?.Invoke(this, new TryAttackEventArgs(this, InteractionKind.HoldStart, tick));
        break;

      case InteractionKind.HoldEnd:
        if (!IsAttackingHold) { // If the entity is not holding the attack button, do nothing.
          break;
        }

        IsAttackingHold = false;
        TryAttackEvent?.Invoke(this, new TryAttackEventArgs(this, InteractionKind.HoldEnd, tick));
        break;
    }
  }

  public void InvokeAfterPositionChangeEvent() {
    AfterPositionChangeEvent?.Invoke(this, new AfterPositionChangeEventArgs(this));
  }

  /// <summary>
  /// Attacks.
  /// </summary>
  /// <param name="kind">The kind of attack</param>
  /// <param name="tick">The tick</param>
  public void Use(InteractionKind kind) {
    switch (kind) {
      case InteractionKind.Click:
        TryUseEvent?.Invoke(this, new TryUseEventArgs(this, InteractionKind.Click));
        break;
    }
  }

  /// <summary>
  /// Sets the entity's orientation to look at the specified position.
  /// </summary>
  /// <param name="position">The position</param>
  public void LookAt(Position<decimal> position) {
    Position<decimal> vector = new(position - EyePosition);
    Orientation = new Orientation(
      Math.Clamp((decimal)Math.Atan2((double)vector.X, (double)vector.Z) * 180 / (decimal)Math.PI, -179.999m, 180),
      Math.Clamp(-(decimal)Math.Atan2((double)vector.Y, (double)Math.Sqrt((double)(vector.X * vector.X + vector.Z * vector.Z))) * 180 / (decimal)Math.PI, -90, 90)
    );

    AfterOrientationChangeEvent?.Invoke(this, new AfterOrientationChangeEventArgs(this));
  }

  /// <summary>
  /// Teleports the entity to the specified position.
  /// </summary>
  /// <param name="position">The position</param>
  public virtual void Teleport(Position<decimal> position) {
    TeleportDestination = position;
  }

  public void Update(Level level, decimal timeInterval, int tick) {
    if (this.TeleportDestination is not null) {
      Position = TeleportDestination;
      this.Velocity = new Velocity(0, 0, 0);
      TeleportDestination = null;

      AfterPositionChangeEvent?.Invoke(this, new AfterPositionChangeEventArgs(this));
    }

    if (HasPhysics is true && IsDead is not true) {
      UpdatePhysics(level, timeInterval, tick);
    }
  }

  /// <summary>
  /// Gets the nearest block that the entity is looking at.
  /// </summary>
  /// <param name="level">The level</param>
  /// <param name="maxDistance">The maximum distance of the block</param>
  /// <returns>The nearest block that the entity is looking at and the intersection point of the ray and the block</returns>
  public (Block? block, Position<decimal>? intersectionPoint) GetNearestLookedAtBlock(Level level, decimal maxDistance) {
    Position<decimal> fowardVector = this.GetForwardVector();
    decimal A = fowardVector.X == 0 ? Epsilon : fowardVector.X;
    decimal B = fowardVector.Y == 0 ? Epsilon : fowardVector.Y;
    decimal C = fowardVector.Z == 0 ? Epsilon : fowardVector.Z;

    decimal x0 = EyePosition.X, y0 = EyePosition.Y, z0 = EyePosition.Z;

    const int AirId = 0;

    var intersectionPointList = VectorMath.GetIntersectionPointList(EyePosition, new Position<decimal>(x0 + A * maxDistance, y0 + B * maxDistance, z0 + C * maxDistance));

    Block? block = null;
    Position<decimal>? intersectionPoint = null;

    foreach (Position<decimal> nowPoint in intersectionPointList) {
      Block nowBlock = level.GetBlock(new Position<int>(
        (int)(nowPoint.X + A * Epsilon),
        (int)(nowPoint.Y + B * Epsilon),
        (int)(nowPoint.Z + C * Epsilon))
       );

      if (nowBlock.TypeId != AirId) {
        intersectionPoint = nowPoint;
        block = nowBlock;
        break;
      }
    }

    return (block, intersectionPoint);
  }

  /// <summary>
  /// Gets the nearest entity that the entity is looking at.
  /// </summary>
  /// <param name="level">The level</param>
  /// <param name="maxDistance">The maximum distance of the entity</param>
  /// <returns>The nearest entity that the entity is looking at and the intersection point of the ray and the entity</returns>
  public (Entity? entity, Position<decimal>? intersectionPoint) GetNearestLookedAtEntity(Level level, decimal maxDistance) {
    // Position<decimal>.GetDistanceBetween();
    // Compute parameter equations of the ray:
    // (x-x0)/a = (y-y0)/b = (z-z0)/c = t => 
    // (1) y = b/a (x-x0) + y0; (2) z = c/a (x-x0) + z0;
    Position<decimal> fowardVector = this.GetForwardVector();
    decimal A = fowardVector.X == 0 ? Epsilon : fowardVector.X;
    decimal B = fowardVector.Y == 0 ? Epsilon : fowardVector.Y;
    decimal C = fowardVector.Z == 0 ? Epsilon : fowardVector.Z;

    decimal x0 = EyePosition.X, y0 = EyePosition.Y, z0 = EyePosition.Z;

    var lineEquations = (decimal t) => {
      return new Position<decimal>(A * t + x0, B * t + y0, C * t + z0);
    };

    List<(Entity entity, Position<decimal> intersectionPoint, decimal minDistance)> TouchedEntitiesAndNearestIntersection = new();
    foreach (Entity entity in level.GetAllEntities()) {
      // has collision box and is not myself
      if (entity.CollisionBox == null || entity.UniqueId == this.UniqueId) {
        continue;
      }

      bool HaveTouched = false;
      AABB collisionBox = entity.CollisionBox!;
      decimal minX = collisionBox.Min.X;
      decimal minY = collisionBox.Min.Y;
      decimal minZ = collisionBox.Min.Z;
      decimal maxX = collisionBox.Max.X;
      decimal maxY = collisionBox.Max.Y;
      decimal maxZ = collisionBox.Max.Z;

      var updateOperation = (Position<decimal> intersectionPosition) => {
        if (!HaveTouched) {
          TouchedEntitiesAndNearestIntersection.Add(
            new(entity, intersectionPosition, Position<decimal>.DistanceBetween(this.EyePosition, intersectionPosition))
          );
        } else {
          decimal currentIntersectionDistance = Position<decimal>.DistanceBetween(EyePosition, intersectionPosition);
          var currentTuple = TouchedEntitiesAndNearestIntersection.Last<(Entity entity, Position<decimal> intersectionPoint, decimal minDistance)>();
          // Compare the distance
          if (currentIntersectionDistance < currentTuple.minDistance) {
            currentTuple.minDistance = currentIntersectionDistance;
          }
        }
        HaveTouched = true;
      };

      // Two X planes
      decimal MinXT = (minX - x0) / A;
      decimal MaxXT = (maxX - x0) / A;
      Position<decimal> MinXIntersectionPosition = lineEquations(MinXT);
      Position<decimal> MaxXIntersectionPosition = lineEquations(MaxXT);
      if (MinXIntersectionPosition.Y < maxY && MinXIntersectionPosition.Y > minY && MinXIntersectionPosition.Z < maxZ && MinXIntersectionPosition.Z > minZ) {
        updateOperation(MinXIntersectionPosition);
      }
      if (MaxXIntersectionPosition.Y < maxY && MaxXIntersectionPosition.Y > minY && MaxXIntersectionPosition.Z < maxZ && MaxXIntersectionPosition.Z > minZ) {
        updateOperation(MaxXIntersectionPosition);
      }

      // Two Y planes
      decimal MinYT = (minY - y0) / A;
      decimal MaxYT = (maxY - y0) / A;
      Position<decimal> MinYIntersectionPosition = lineEquations(MinYT);
      Position<decimal> MaxYIntersectionPosition = lineEquations(MaxYT);
      if (MinYIntersectionPosition.X < maxX && MinYIntersectionPosition.X > minX && MinYIntersectionPosition.Z < maxZ && MinYIntersectionPosition.Z > minZ) {
        updateOperation(MinYIntersectionPosition);
      }
      if (MaxYIntersectionPosition.X < maxY && MaxYIntersectionPosition.X > minY && MaxYIntersectionPosition.Z < maxZ && MaxYIntersectionPosition.Z > minZ) {
        updateOperation(MaxYIntersectionPosition);
      }

      // Two Z planes
      decimal MinZT = (minY - y0) / A;
      decimal MaxZT = (maxY - y0) / A;
      Position<decimal> MinZIntersectionPosition = lineEquations(MinZT);
      Position<decimal> MaxZIntersectionPosition = lineEquations(MaxZT);
      if (MinZIntersectionPosition.X < maxX && MinZIntersectionPosition.X > minX && MinZIntersectionPosition.Y < maxY && MinZIntersectionPosition.Y > minY) {
        updateOperation(MinZIntersectionPosition);
      }
      if (MaxZIntersectionPosition.X < maxY && MaxZIntersectionPosition.X > minY && MaxZIntersectionPosition.Y < maxY && MaxZIntersectionPosition.Y > minY) {
        updateOperation(MaxZIntersectionPosition);
      }
    }
    // Find the first touched entity
    decimal totalMinDistance = maxDistance;
    Position<decimal>? targetIntersection = null;
    Entity? targetEntity = null;
    foreach (var item in TouchedEntitiesAndNearestIntersection) {
      if (totalMinDistance > item.minDistance) {
        targetEntity = item.entity;
        targetIntersection = item.intersectionPoint;
      }
    }
    return (targetEntity, targetIntersection);
  }

  private Position<decimal> GetForwardVector() {
    // Normalized forward vector: (A, B, C)
    decimal A = (decimal)(Math.Cos(-(double)this.Orientation.Pitch * Math.PI / 180) * Math.Sin((double)this.Orientation.Yaw * Math.PI / 180));
    decimal B = (decimal)(Math.Sin(-(double)this.Orientation.Pitch * Math.PI / 180));
    decimal C = (decimal)(Math.Cos(-(double)this.Orientation.Pitch * Math.PI / 180) * Math.Cos((double)this.Orientation.Yaw * Math.PI / 180));

    return new Position<decimal>(A, B, C);
  }
  #endregion
}
