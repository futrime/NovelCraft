using System.Text.Json;
using NovelCraft.Server.Recorder;
using Xunit.Abstractions;

namespace NovelCraft.Server.Tests.Recorder;

public class RecorderTests : NovelCraft.Server.Tests.TestsType {
  [Fact]
  public void TestRecorder() {
    NovelCraft.Server.Recorder.Recorder recorder = new("tests/records");

    recorder.Record(new AfterEntityCreateEventRecord() {
      Tick = 1,
      Data = new() {
        CreationList = new() {
          new() {
            EntityTypeId = 1,
            UniqueId = 2,
            Position = new() {
              X = 3,
              Y = 4,
              Z = 5
            },
            Orientation = new() {
              Yaw = 6,
              Pitch = 7
            }
          }
        }
      }
    });

    Assert.Equal(
        @"{""type"":""record"",""records"":[{""identifier"":""after_entity_create"",""tick"":1,""type"":""event"",""data"":{""creation_list"":[{""entity_type_id"":1,""unique_id"":2,""position"":{""x"":3,""y"":4,""z"":5},""orientation"":{""yaw"":6,""pitch"":7}}]}}]}",
        JsonSerializer.Serialize(recorder.Json)
    );
  }

  public RecorderTests(ITestOutputHelper output) : base(output) { }
}