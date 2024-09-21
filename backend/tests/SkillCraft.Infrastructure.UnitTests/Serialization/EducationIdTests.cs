using SkillCraft.Domain.Educations;
using SkillCraft.Infrastructure.Converters;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class EducationIdTests
{
  private readonly JsonSerializerOptions _options = new();

  public EducationIdTests()
  {
    _options.Converters.Add(new EducationIdConverter());
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    string json = string.Concat('"', EducationId.NewId(), '"');
    EducationId educationId = JsonSerializer.Deserialize<EducationId>(json, _options);
    Assert.Equal(json.Trim('"'), educationId.Value);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    EducationId educationId = JsonSerializer.Deserialize<EducationId>(json, _options);
    Assert.Equal(string.Empty, educationId.Value);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    EducationId educationId = EducationId.NewId();
    string json = JsonSerializer.Serialize(educationId, _options);
    Assert.Equal(string.Concat('"', educationId, '"'), json);
  }
}
