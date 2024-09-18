using SkillCraft.Domain.Languages;
using SkillCraft.Infrastructure.Converters;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class LanguageIdTests
{
  private readonly JsonSerializerOptions _options = new();

  public LanguageIdTests()
  {
    _options.Converters.Add(new LanguageIdConverter());
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    string json = string.Concat('"', LanguageId.NewId(), '"');
    LanguageId languageId = JsonSerializer.Deserialize<LanguageId>(json, _options);
    Assert.Equal(json.Trim('"'), languageId.Value);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    LanguageId languageId = JsonSerializer.Deserialize<LanguageId>(json, _options);
    Assert.Equal(string.Empty, languageId.Value);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    LanguageId languageId = LanguageId.NewId();
    string json = JsonSerializer.Serialize(languageId, _options);
    Assert.Equal(string.Concat('"', languageId, '"'), json);
  }
}
