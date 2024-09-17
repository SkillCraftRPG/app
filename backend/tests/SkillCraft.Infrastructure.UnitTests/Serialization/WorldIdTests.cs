using SkillCraft.Domain.Worlds;
using SkillCraft.Infrastructure.Converters;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class WorldIdTests
{
  private readonly JsonSerializerOptions _options = new();

  public WorldIdTests()
  {
    _options.Converters.Add(new WorldIdConverter());
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    string json = string.Concat('"', WorldId.NewId(), '"');
    WorldId worldId = JsonSerializer.Deserialize<WorldId>(json, _options);
    Assert.Equal(json.Trim('"'), worldId.Value);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    WorldId worldId = JsonSerializer.Deserialize<WorldId>(json, _options);
    Assert.Equal(string.Empty, worldId.Value);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    WorldId worldId = WorldId.NewId();
    string json = JsonSerializer.Serialize(worldId, _options);
    Assert.Equal(string.Concat('"', worldId, '"'), json);
  }
}
