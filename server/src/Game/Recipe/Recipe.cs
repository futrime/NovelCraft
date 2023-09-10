namespace NovelCraft.Server.Game;

public class Recipe {
  #region Nested classes, enums, delegates and events
  public enum RecipeType { Crafting }
  #endregion


  #region Fields and properties
  public List<(int ItemTypeId, int Count)> Results => (
    from result in _definition.Recipe.Result select (result.ItemTypeId, result.Count)).ToList();

  private RecipeDefinition _definition;
  #endregion


  #region Constructors and finalizers
  public Recipe(RecipeDefinition definition) {
    _definition = definition;
  }
  #endregion


  #region Methods
  /// <summary>
  /// Checks if the recipe can be crafted with the given ingredients.
  /// </summary>
  /// <param name="ingredients">A list of item type IDs.</param>
  /// <returns>True if the recipe can be crafted with the given ingredients, false otherwise.</returns>
  public bool CanCraftWith(List<int?> ingredients) {
    if (ingredients.Count != _definition.Recipe.Ingredients.Count) {
      return false;
    }

    for (int i = 0; i < ingredients.Count; i++) {
      if (ingredients[i] != _definition.Recipe.Ingredients[i]) {
        return false;
      }
    }

    return true;
  }
  #endregion
}