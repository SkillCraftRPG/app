using SkillCraft.Domain.Lineages;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class LineageAttributesTests
{
  private readonly JsonSerializerOptions _options = new();

  public LineageAttributesTests()
  {
    _options.Converters.Add(new JsonStringEnumConverter());
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    string json = @"{""Agility"":0,""Coordination"":0,""Intellect"":0,""Presence"":1,""Sensitivity"":0,""Spirit"":1,""Vigor"":0,""Extra"":1}";
    AttributeBonuses? attributes = JsonSerializer.Deserialize<AttributeBonuses>(json, _options);
    Assert.NotNull(attributes);
    Assert.Equal(0, attributes.Agility);
    Assert.Equal(0, attributes.Coordination);
    Assert.Equal(0, attributes.Intellect);
    Assert.Equal(1, attributes.Presence);
    Assert.Equal(0, attributes.Sensitivity);
    Assert.Equal(1, attributes.Spirit);
    Assert.Equal(0, attributes.Vigor);
    Assert.Equal(1, attributes.Extra);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    AttributeBonuses? attributes = JsonSerializer.Deserialize<AttributeBonuses>(json, _options);
    Assert.Null(attributes);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    AttributeBonuses attributes = new(agility: 0, coordination: 0, intellect: 0, presence: 1, sensitivity: 0, spirit: 1, vigor: 0, extra: 1);
    string json = JsonSerializer.Serialize(attributes, _options);
    Assert.Equal(@"{""Agility"":0,""Coordination"":0,""Intellect"":0,""Presence"":1,""Sensitivity"":0,""Spirit"":1,""Vigor"":0,""Extra"":1}", json);
  }
}
