using SkillCraft.Domain.Aspects;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class AspectAttributesTests
{
  private readonly JsonSerializerOptions _options = new();

  public AspectAttributesTests()
  {
    _options.Converters.Add(new JsonStringEnumConverter());
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    string json = @"{""Mandatory1"":""Intellect"",""Mandatory2"":""Sensitivity"",""Optional1"":""Agility"",""Optional2"":""Coordination""}";
    Attributes? attributes = JsonSerializer.Deserialize<Attributes>(json, _options);
    Assert.NotNull(attributes);
    Assert.Equal(Attribute.Intellect, attributes.Mandatory1);
    Assert.Equal(Attribute.Sensitivity, attributes.Mandatory2);
    Assert.Equal(Attribute.Agility, attributes.Optional1);
    Assert.Equal(Attribute.Coordination, attributes.Optional2);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    Attributes? attributes = JsonSerializer.Deserialize<Attributes>(json, _options);
    Assert.Null(attributes);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    Attributes attributes = new(Attribute.Intellect, Attribute.Sensitivity, Attribute.Agility, Attribute.Coordination);
    string json = JsonSerializer.Serialize(attributes, _options);
    Assert.Equal(@"{""Mandatory1"":""Intellect"",""Mandatory2"":""Sensitivity"",""Optional1"":""Agility"",""Optional2"":""Coordination""}", json);
  }
}
