using SkillCraft.Contracts;
using SkillCraft.Domain.Aspects;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class SkillsTests
{
  private readonly JsonSerializerOptions _options = new();

  public SkillsTests()
  {
    _options.Converters.Add(new JsonStringEnumConverter());
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    string json = @"{""Discounted1"":""Investigation"",""Discounted2"":""Medicine""}";
    Skills? skills = JsonSerializer.Deserialize<Skills>(json, _options);
    Assert.NotNull(skills);
    Assert.Equal(Skill.Investigation, skills.Discounted1);
    Assert.Equal(Skill.Medicine, skills.Discounted2);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    Skills? skills = JsonSerializer.Deserialize<Skills>(json, _options);
    Assert.Null(skills);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    Skills skills = new(Skill.Investigation, Skill.Medicine);
    string json = JsonSerializer.Serialize(skills, _options);
    Assert.Equal(@"{""Discounted1"":""Investigation"",""Discounted2"":""Medicine""}", json);
  }
}
