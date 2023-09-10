namespace NovelCraft.Sdk {

  internal class ItemStack : IItemStack {
    public int Count { get; set; }
    public int TypeId { get; }


    public ItemStack(int typeId, int count) {
      TypeId = typeId;
      Count = count;
    }
  }

}