using SkillCraft.Domain.Aspects;
using SkillCraft.Domain.Worlds;
using SkillCraft.Infrastructure.Converters;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class AspectIdTests
{
  private readonly JsonSerializerOptions _options = new();

  private readonly AspectId _id = new(WorldId.NewId());

  public AspectIdTests()
  {
    _options.Converters.Add(new AspectIdConverter());
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    string json = string.Concat('"', _id, '"');
    AspectId aspectId = JsonSerializer.Deserialize<AspectId>(json, _options);
    Assert.Equal(json.Trim('"'), aspectId.Value);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    AspectId aspectId = JsonSerializer.Deserialize<AspectId>(json, _options);
    Assert.Equal(string.Empty, aspectId.Value);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    string json = JsonSerializer.Serialize(_id, _options);
    Assert.Equal(string.Concat('"', _id, '"'), json);
  }
}
