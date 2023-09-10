using NovelCraft.Server.Game;
using Xunit.Abstractions;

namespace NovelCraft.Server.Tests.Game;

public class VectorMathTests : NovelCraft.Server.Tests.TestsType {
  [Fact]
  public void TestGetIntersectionPointList() {
    var fromPosition = new Position<decimal>(0, 0, 0);
    var toPosition = new Position<decimal>(1, 2, 3);

    var list = VectorMath.GetIntersectionPointList(fromPosition, toPosition);

    Assert.Equal(5, list.Count);

    var expectedList = new List<Position<decimal>> {
      new Position<decimal>(0, 0, 0),
      new Position<decimal>(1M/3, 2M/3, 1),
      new Position<decimal>(1M/2, 1, 3M/2),
      new Position<decimal>(2M/3, 4M/3, 2),
      new Position<decimal>(1, 2, 3),
    };

    for (int i = 0; i < Math.Min(expectedList.Count, list.Count); i++) {
      Assert.Equal(expectedList[i].X, list[i].X, 6);
      Assert.Equal(expectedList[i].Y, list[i].Y, 6);
      Assert.Equal(expectedList[i].Z, list[i].Z, 6);
    }
  }

  public VectorMathTests(ITestOutputHelper output) : base(output) { }
}
