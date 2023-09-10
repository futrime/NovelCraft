using Xunit.Abstractions;

namespace NovelCraft.Server.Tests;

public abstract class TestsType {
  private readonly ITestOutputHelper _output;

  protected TestsType(ITestOutputHelper output) {
    _output = output;
  }

  protected void Log(string message) {
    _output.WriteLine(message);
  }
}