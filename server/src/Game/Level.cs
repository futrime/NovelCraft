using System.Text.Json.Nodes;

namespace NovelCraft.Server.Game;

/// <summary>
/// Level class is the class that represents a level.
/// </summary>
public class Level {
  #region Fields and properties
  public JsonNode Json {
    get {
      List<LevelData.SectionType> sections = new();
      foreach (Section section in _blockSource.GetAllSections()) {
        sections.Add(new LevelData.SectionType() {
          X = section.Position.X,
          Y = section.Position.Y,
          Z = section.Position.Z,
          Blocks = section.GetAllBlockIds()
        });
      }

      List<LevelData.EntityType> entities = new();
      foreach (Entity entity in _entitySource.GetAllEntities()) {
        entities.Add(new LevelData.EntityType() {
          EntityId = entity.TypeId,
          UniqueId = entity.UniqueId,
          Position = new() {
            X = entity.Position.X,
            Y = entity.Position.Y,
            Z = entity.Position.Z
          },
          Orientation = new() {
            Yaw = entity.Orientation.Yaw,
            Pitch = entity.Orientation.Pitch
          }
        });
      }

      LevelData level_data = new() {
        Sections = sections,
        Entities = entities
      };

      return level_data.Json;
    }
  }

  private BlockSource _blockSource = new();
  private EntitySource _entitySource = new();
  #endregion


  #region Constructors and finalizers
  /// <summary>
  /// Create a new empty level.
  /// </summary>
  public Level() {
    // Empty.
  }

  /// <summary>
  /// Create a new level from a JSON object.
  /// </summary>
  /// <param name="jsonString">The JSON object</param>
  public Level(LevelData levelData) {
    List<Section> sectionList = new();

    foreach (LevelData.SectionType section in levelData.Sections) {
      Position<int> sectionPosition = new() {
        X = (int)section.X,
        Y = (int)section.Y,
        Z = (int)section.Z
      };

      List<int> blockList = new(4096);

      for (int i = 0; i < 4096; i++) {
        blockList.Add((int)section.Blocks[i]);
      }

      sectionList.Add(new Section(sectionPosition, blockList));
    }

    _blockSource = new BlockSource(sectionList);

    // TODO: Load entities.
  }
  #endregion


  #region Methods
  /// <summary>
  /// Checks if the box collides with a block in the level.
  /// </summary>
  public bool CheckCollisionWithAnyBlock(AABB box) {
    Position<int> min = new() {
      X = (int)Math.Floor(box.Min.X),
      Y = (int)Math.Floor(box.Min.Y),
      Z = (int)Math.Floor(box.Min.Z)
    };

    Position<int> max = new() {
      X = (int)Math.Floor(box.Max.X),
      Y = (int)Math.Floor(box.Max.Y),
      Z = (int)Math.Floor(box.Max.Z)
    };

    for (int x = min.X; x <= max.X; x++) {
      for (int y = min.Y; y <= max.Y; y++) {
        for (int z = min.Z; z <= max.Z; z++) {
          Block block = GetBlock(new Position<int>(x, y, z));
          if (block.CollisionBox is not null && AABB.Intersect(box, block.CollisionBox)) {
            return true;
          }
        }
      }
    }

    return false;
  }

  /// <summary>
  /// Checks if the box contacts with blocks in the level.
  /// </summary>
  public Dictionary<DirectionKind, bool> CheckContaction(AABB box) {
    const decimal Epsilon = 1e-6m;

    Dictionary<DirectionKind, bool> contactionDict = new() {
      { DirectionKind.North, false },
      { DirectionKind.South, false },
      { DirectionKind.East, false },
      { DirectionKind.West, false },
      { DirectionKind.Up, false },
      { DirectionKind.Down, false }
    };

    foreach (DirectionKind direction in Enum.GetValues(typeof(DirectionKind))) {
      AABB boxToTest = direction switch {
        DirectionKind.North => new(new Position<decimal> {
          X = (box.Min.X),
          Y = box.Min.Y,
          Z = (box.Min.Z + Epsilon)
        }, box.Size),
        DirectionKind.South => new(new Position<decimal> {
          X = (box.Min.X),
          Y = box.Min.Y,
          Z = (box.Min.Z - Epsilon)
        }, box.Size),
        DirectionKind.East => new(new Position<decimal> {
          X = (box.Min.X + Epsilon),
          Y = box.Min.Y,
          Z = (box.Min.Z)
        }, box.Size),
        DirectionKind.West => new(new Position<decimal> {
          X = (box.Min.X - Epsilon),
          Y = box.Min.Y,
          Z = (box.Min.Z)
        }, box.Size),
        DirectionKind.Up => new(new Position<decimal> {
          X = (box.Min.X),
          Y = (box.Min.Y + Epsilon),
          Z = (box.Min.Z)
        }, box.Size),
        DirectionKind.Down => new(new Position<decimal> {
          X = (box.Min.X),
          Y = (box.Min.Y - Epsilon),
          Z = (box.Min.Z)
        }, box.Size),

        _ => throw new InvalidOperationException("Invalid direction.")
      };

      contactionDict[direction] = CheckCollisionWithAnyBlock(boxToTest);
    }

    return contactionDict;
  }

  /// <summary>
  /// Creates an entity.
  /// </summary>
  /// <param name="typeId">The type ID of the entity</param>
  /// <param name="position">The position of the entity</param>
  /// <param name="tickCreated">The tick when the entity was created</param>
  /// <returns>The unique ID of the entity</returns>
  public int CreateEntity(int typeId, Position<decimal> position, int tickCreated) {
    return _entitySource.CreateEntity(typeId, position, tickCreated);
  }

  /// <summary>
  /// Creates an item entity.
  /// </summary>
  /// <param name="itemStack">The item stack</param>
  /// <param name="position">The position of the entity</param>
  /// <param name="tickCreated">The tick when the entity was created</param>
  /// <returns>The unique ID of the entity</returns>
  public int CreateItemEntity(ItemStack itemStack, Position<decimal> position, int tickCreated) {
    return _entitySource.CreateItemEntity(itemStack, position, tickCreated);
  }

  /// <summary>
  /// Gets all entities.
  /// </summary>
  /// <returns>The list of entities</returns>
  public List<Entity> GetAllEntities() {
    return _entitySource.GetAllEntities();
  }

  /// <summary>
  /// Gets all players.
  /// </summary>
  /// <returns>The list of players</returns>
  public List<Player> GetAllPlayers() {
    return (from entity in GetAllEntities() where entity is NovelCraft.Server.Game.Player player select (Player)entity).ToList();
  }

  /// <summary>
  /// Gets the block at the specified position.
  /// </summary>
  /// <param name="position">The position</param>
  /// <returns>The block</returns>
  public Block GetBlock(Position<int> position) {
    return _blockSource.GetBlock(position);
  }

  /// <summary>
  /// Gets the block position range of the level.
  /// </summary>
  /// <returns>The block position range.</returns>
  public (Position<int> Min, Position<int> Max) GetBlockPositionRange() {
    return _blockSource.GetBlockPositionRange();
  }

  /// <summary>
  /// Gets the entity with the specified unique ID.
  /// </summary>
  /// <param name="uniqueId">The unique ID</param>
  /// <returns>The entity</returns>
  public Entity? GetEntity(int uniqueId) {
    return _entitySource.GetEntity(uniqueId);
  }

  /// <summary>
  /// Gets the section at the specified position.
  /// </summary>
  /// <param name="position">The position</param>
  /// <returns>The section</returns>
  public Section GetSectionAt(Position<int> position) {
    return _blockSource.GetSection(position);
  }

  /// <summary>
  /// Regiters a block definition.
  /// </summary>
  /// <param name="definition">The definition</param>
  public void RegisterDefinition(BlockDefinition definition) {
    _blockSource.RegisterDefinition(definition);
  }

  /// <summary>
  /// Regiters an entity definition.
  /// </summary>
  /// <param name="definition">The definition</param>
  public void RegisterDefinition(EntityDefinition definition) {
    _entitySource.RegisterDefinition(definition);
  }

  /// <summary>
  /// Removes the entity with the specified unique ID.
  /// </summary>
  /// <param name="uniqueId">The unique ID</param>
  public void RemoveEntity(int uniqueId) {
    _entitySource.RemoveEntity(uniqueId);
  }

  /// <summary>
  /// Sets the block at the specified position.
  /// </summary>
  /// <param name="position">The position</param>
  /// <param name="blockId">The block ID</param>
  public void SetBlock(Position<int> position, int blockId) {
    _blockSource.SetBlock(position, blockId);
  }

  /// <summary>
  /// Sets the block at the specified position.
  /// </summary>
  /// <param name="position">The position</param>
  /// <param name="block">The block</param>
  public void SetBlock(Position<int> position, Block block) {
    _blockSource.SetBlock(position, block);
  }
  #endregion
}
