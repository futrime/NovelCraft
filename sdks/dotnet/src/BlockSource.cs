namespace NovelCraft.Sdk;

internal class BlockSource : IBlockSource {
  public IBlock? this[IPosition<int> position] {
    get {
      var section = GetSection(position);
      if (section is null) {
        return null;
      }

      return section[new Position<int>(position.X - section.Position.X, position.Y - section.Position.Y, position.Z - section.Position.Z)];
    }

    set {
      if (value is null) {
        throw new ArgumentNullException(nameof(value));
      }

      var section = GetSection(position);
      if (section is null) {
        throw new ArgumentException("The section is not loaded.");
      }

      section[new Position<int>(position.X - section.Position.X, position.Y - section.Position.Y, position.Z - section.Position.Z)] = value;
    }
  }

  public int Count => _sectionDictionary.Count * 16 * 16 * 16;

  private Dictionary<IPosition<int>, Section> _sectionDictionary = new();


  public void AddSection(Section section) {
    _sectionDictionary[section.Position] = section;
  }

  public void Clear() {
    _sectionDictionary.Clear();
  }

  public void RemoveSection(IPosition<int> position) {
    // Check if the position is valid.
    if (position.X % 16 != 0 || position.Y % 16 != 0 || position.Z % 16 != 0) {
      throw new ArgumentException("The position should be multiples of 16.");
    }

    if (_sectionDictionary.ContainsKey(position)) {
      _sectionDictionary.Remove(position);
    }
  }

  private Section? GetSection(IPosition<int> position) {
    Position<int> sectionPosition = new Position<int> {
      X = 16 * (int)Math.Floor(position.X / 16.0),
      Y = 16 * (int)Math.Floor(position.Y / 16.0),
      Z = 16 * (int)Math.Floor(position.Z / 16.0)
    };

    return _sectionDictionary.ContainsKey(sectionPosition) ? _sectionDictionary[sectionPosition] : null;
  }
}