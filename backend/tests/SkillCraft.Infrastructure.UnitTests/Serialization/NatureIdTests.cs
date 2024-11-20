using SkillCraft.Domain.Natures;
using SkillCraft.Domain.Worlds;
using SkillCraft.Infrastructure.Converters;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class NatureIdTests
{
  private readonly JsonSerializerOptions _options = new();

  private readonly NatureId _id = new(WorldId.NewId());

  public NatureIdTests()
  {
    _options.Converters.Add(new NatureIdConverter());
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    string json = string.Concat('"', _id, '"');
    NatureId natureId = JsonSerializer.Deserialize<NatureId>(json, _options);
    Assert.Equal(json.Trim('"'), natureId.Value);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    NatureId natureId = JsonSerializer.Deserialize<NatureId>(json, _options);
    Assert.Equal(string.Empty, natureId.Value);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    string json = JsonSerializer.Serialize(_id, _options);
    Assert.Equal(string.Concat('"', _id, '"'), json);
  }
}
