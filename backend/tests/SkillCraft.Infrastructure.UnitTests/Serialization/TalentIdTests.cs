using SkillCraft.Domain.Talents;
using SkillCraft.Infrastructure.Converters;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class TalentIdTests
{
  private readonly JsonSerializerOptions _options = new();

  public TalentIdTests()
  {
    _options.Converters.Add(new TalentIdConverter());
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    string json = string.Concat('"', TalentId.NewId(), '"');
    TalentId talentId = JsonSerializer.Deserialize<TalentId>(json, _options);
    Assert.Equal(json.Trim('"'), talentId.Value);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    TalentId talentId = JsonSerializer.Deserialize<TalentId>(json, _options);
    Assert.Equal(string.Empty, talentId.Value);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    TalentId talentId = TalentId.NewId();
    string json = JsonSerializer.Serialize(talentId, _options);
    Assert.Equal(string.Concat('"', talentId, '"'), json);
  }
}
