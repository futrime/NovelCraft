namespace NovelCraft.Server.Game;

public class RecipeSource {
  #region Fields and properties
  private ItemStackFactory _itemStackFactory;
  private List<Recipe> _recipeList = new();
  #endregion


  #region Constructors and finalizers
  public RecipeSource(ItemStackFactory itemStackFactory) {
    _itemStackFactory = itemStackFactory;
  }
  #endregion


  #region Methods
  /// <summary>
  /// Crafts a list of items from the given ingredients. Null if failed to craft.
  /// </summary>
  /// <param name="ingredients">A list of ingredients.</param>
  /// <returns>A list of items.</returns>
  public List<ItemStack>? Craft(List<ItemStack?> ingredients) {
    foreach (Recipe recipe in _recipeList) {
      if (recipe.CanCraftWith((from ingredient in ingredients select ingredient?.TypeId).ToList())) {
        List<ItemStack> results = (
          from result in recipe.Results select _itemStackFactory.CreateItemStack(result.ItemTypeId, result.Count)).ToList();

        return results;
      }
    }

    return null;
  }

  /// <summary>
  /// Registers a recipe definition.
  /// </summary>
  /// <param name="definition">A recipe definition.</param>
  public void RegisterDefinition(RecipeDefinition definition) {
    _recipeList.Add(new Recipe(definition));
  }

  /// <summary>
  /// Registers a list of recipe definitions.
  /// </summary>
  /// <param name="definitionList">A list of recipe definitions.</param>
  public void RegisterDefinitions(List<RecipeDefinition> definitionList) {
    foreach (RecipeDefinition definition in definitionList) {
      RegisterDefinition(definition);
    }
  }
  #endregion
}