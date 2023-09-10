using System.Collections;

namespace NovelCraft.Sdk;

internal class EntitySource : IEntitySource {
  public IEntity? this[int uniqueId] {
    get {
      return _entityDictionary.ContainsKey(uniqueId) ? _entityDictionary[uniqueId] : null;
    }

    set {
      if (value is null) {
        throw new ArgumentNullException(nameof(value));
      }

      if (uniqueId != value.UniqueId) {
        throw new ArgumentException("The uniqueId of the entity does not match the key.");
      }

      _entityDictionary[uniqueId] = value;
    }
  }

  private Dictionary<int, IEntity> _entityDictionary = new();


  public void Clear() {
    _entityDictionary.Clear();
  }

  public List<IEntity> GetAllEntities() {
    return _entityDictionary.Values.ToList();
  }
  public void AddEntity(IEntity entity) {
    if (_entityDictionary.ContainsKey(entity.UniqueId) == true) {
      return;
    }

    _entityDictionary.Add(entity.UniqueId, entity);
  }

  public void RemoveEntity(int uniqueId) {
    if (_entityDictionary.ContainsKey(uniqueId) == false) {
      return;
    }
    _entityDictionary.Remove(uniqueId);
  }

  IEnumerator<IEntity> IEnumerable<IEntity>.GetEnumerator() {
    return _entityDictionary.Values.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() {
    return _entityDictionary.Values.GetEnumerator();
  }

}