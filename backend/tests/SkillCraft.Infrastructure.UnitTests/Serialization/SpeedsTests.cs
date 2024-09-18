using SkillCraft.Domain.Lineages;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class SpeedsTests
{
  private readonly JsonSerializerOptions _options = new();

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    string json = @"{""Walk"":6,""Climb"":3,""Swim"":4,""Fly"":5,""Hover"":0,""Burrow"":1}";
    Speeds? speeds = JsonSerializer.Deserialize<Speeds>(json, _options);
    Assert.NotNull(speeds);
    Assert.Equal(6, speeds.Walk);
    Assert.Equal(3, speeds.Climb);
    Assert.Equal(4, speeds.Swim);
    Assert.Equal(5, speeds.Fly);
    Assert.Equal(0, speeds.Hover);
    Assert.Equal(1, speeds.Burrow);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    Speeds? speeds = JsonSerializer.Deserialize<Speeds>(json, _options);
    Assert.Null(speeds);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    Speeds speeds = new(walk: 6, climb: 3, swim: 4, fly: 5, hover: 0, burrow: 1);
    string json = JsonSerializer.Serialize(speeds, _options);
    Assert.Equal(@"{""Walk"":6,""Climb"":3,""Swim"":4,""Fly"":5,""Hover"":0,""Burrow"":1}", json);
  }
}
