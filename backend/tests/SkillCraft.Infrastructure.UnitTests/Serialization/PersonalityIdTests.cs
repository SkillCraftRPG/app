using SkillCraft.Domain.Personalities;
using SkillCraft.Infrastructure.Converters;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class PersonalityIdTests
{
  private readonly JsonSerializerOptions _options = new();

  public PersonalityIdTests()
  {
    _options.Converters.Add(new PersonalityIdConverter());
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    string json = string.Concat('"', PersonalityId.NewId(), '"');
    PersonalityId personalityId = JsonSerializer.Deserialize<PersonalityId>(json, _options);
    Assert.Equal(json.Trim('"'), personalityId.Value);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    PersonalityId personalityId = JsonSerializer.Deserialize<PersonalityId>(json, _options);
    Assert.Equal(string.Empty, personalityId.Value);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    PersonalityId personalityId = PersonalityId.NewId();
    string json = JsonSerializer.Serialize(personalityId, _options);
    Assert.Equal(string.Concat('"', personalityId, '"'), json);
  }
}
