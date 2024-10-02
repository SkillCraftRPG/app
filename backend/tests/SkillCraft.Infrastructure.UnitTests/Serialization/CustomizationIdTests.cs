using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Worlds;
using SkillCraft.Infrastructure.Converters;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class CustomizationIdTests
{
  private readonly JsonSerializerOptions _options = new();

  private readonly CustomizationId _id = new(WorldId.NewId());

  public CustomizationIdTests()
  {
    _options.Converters.Add(new CustomizationIdConverter());
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    string json = string.Concat('"', _id, '"');
    CustomizationId customizationId = JsonSerializer.Deserialize<CustomizationId>(json, _options);
    Assert.Equal(json.Trim('"'), customizationId.Value);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    CustomizationId customizationId = JsonSerializer.Deserialize<CustomizationId>(json, _options);
    Assert.Equal(string.Empty, customizationId.Value);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    string json = JsonSerializer.Serialize(_id, _options);
    Assert.Equal(string.Concat('"', _id, '"'), json);
  }
}
