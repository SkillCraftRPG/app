using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Worlds;
using SkillCraft.Infrastructure.Converters;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class LanguageIdTests
{
  private readonly JsonSerializerOptions _options = new();

  private readonly LanguageId _id = new(WorldId.NewId());

  public LanguageIdTests()
  {
    _options.Converters.Add(new LanguageIdConverter());
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    string json = string.Concat('"', _id, '"');
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
    string json = JsonSerializer.Serialize(_id, _options);
    Assert.Equal(string.Concat('"', _id, '"'), json);
  }
}
