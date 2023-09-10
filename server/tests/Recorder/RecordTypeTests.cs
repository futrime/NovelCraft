using System.Text.Json;
using NovelCraft.Server.Recorder;
using Xunit.Abstractions;

namespace NovelCraft.Server.Tests.Recorder;

public class RecordTypeTests : NovelCraft.Server.Tests.TestsType {
  [Fact]
  public void TestAfterEntityCreateEventRecordType() {
    AfterEntityCreateEventRecord rec1 = new() {
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
    };

    Assert.Equal(
      @"{""identifier"":""after_entity_create"",""tick"":1,""type"":""event"",""data"":{""creation_list"":[{""entity_type_id"":1,""unique_id"":2,""position"":{""x"":3,""y"":4,""z"":5},""orientation"":{""yaw"":6,""pitch"":7}}]}}",
      JsonSerializer.Serialize(rec1)
    );

    AfterEntityCreateEventRecord rec2 = new() {
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
            },
            ItemTypeId = 8
          }
        }
      }
    };

    Assert.Equal(
      @"{""identifier"":""after_entity_create"",""tick"":1,""type"":""event"",""data"":{""creation_list"":[{""entity_type_id"":1,""unique_id"":2,""position"":{""x"":3,""y"":4,""z"":5},""orientation"":{""yaw"":6,""pitch"":7},""item_type_id"":8}]}}",
      JsonSerializer.Serialize(rec2)
    );
  }

  [Fact]
  public void TestAfterEntityPositionChangeEventRecordType() {
    AfterEntityPositionChangeEventRecord rec1 = new() {
      Tick = 1,
      Data = new() {
        ChangeList = new() {
          new() {
            UniqueId = 2,
            Position = new() {
              X = 3,
              Y = 4,
              Z = 5
            }
          }
        }
      }
    };

    Assert.Equal(
      @"{""identifier"":""after_entity_position_change"",""tick"":1,""type"":""event"",""data"":{""change_list"":[{""unique_id"":2,""position"":{""x"":3,""y"":4,""z"":5}}]}}",
      JsonSerializer.Serialize(rec1)
    );
  }

  public RecordTypeTests(ITestOutputHelper output) : base(output) { }
}