using SkillCraft.Domain.Characters;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class BaseAttributesTests
{
  private readonly JsonSerializerOptions _options = new();

  public BaseAttributesTests()
  {
    _options.Converters.Add(new JsonStringEnumConverter());
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    string json = string.Concat('{'
      + @"""Agility"":9,""Coordination"":9,""Intellect"":6,""Presence"":10,""Sensitivity"":7,""Spirit"":6,""Vigor"":10,"
      + @"""Best"":""Agility"",""Worst"":""Sensitivity"",""Mandatory"":[""Agility"",""Vigor""],""Optional"":[""Coordination"",""Vigor""],"
      + @"""Extra"":[""Agility"",""Vigor""]" + '}');
    BaseAttributes? baseAttributes = JsonSerializer.Deserialize<BaseAttributes>(json, _options);
    Assert.NotNull(baseAttributes);
    Assert.Equal(9, baseAttributes.Agility);
    Assert.Equal(9, baseAttributes.Coordination);
    Assert.Equal(6, baseAttributes.Intellect);
    Assert.Equal(10, baseAttributes.Presence);
    Assert.Equal(7, baseAttributes.Sensitivity);
    Assert.Equal(6, baseAttributes.Spirit);
    Assert.Equal(10, baseAttributes.Vigor);
    Assert.Equal(Attribute.Agility, baseAttributes.Best);
    Assert.Equal(Attribute.Sensitivity, baseAttributes.Worst);
    Assert.Equal([Attribute.Agility, Attribute.Vigor], baseAttributes.Mandatory);
    Assert.Equal([Attribute.Coordination, Attribute.Vigor], baseAttributes.Optional);
    Assert.Equal([Attribute.Agility, Attribute.Vigor], baseAttributes.Extra);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    BaseAttributes? baseAttributes = JsonSerializer.Deserialize<BaseAttributes>(json, _options);
    Assert.Null(baseAttributes);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    BaseAttributes baseAttributes = new(agility: 9, coordination: 9, intellect: 6, presence: 10, sensitivity: 7, spirit: 6, vigor: 10,
      best: Attribute.Agility, worst: Attribute.Sensitivity, mandatory: [Attribute.Agility, Attribute.Vigor],
      optional: [Attribute.Coordination, Attribute.Vigor], extra: [Attribute.Agility, Attribute.Vigor]);
    string json = JsonSerializer.Serialize(baseAttributes, _options);
    Assert.Equal(string.Concat('{'
      + @"""Agility"":9,""Coordination"":9,""Intellect"":6,""Presence"":10,""Sensitivity"":7,""Spirit"":6,""Vigor"":10,"
      + @"""Best"":""Agility"",""Worst"":""Sensitivity"",""Mandatory"":[""Agility"",""Vigor""],""Optional"":[""Coordination"",""Vigor""],"
      + @"""Extra"":[""Agility"",""Vigor""]" + '}'), json);
  }
}
