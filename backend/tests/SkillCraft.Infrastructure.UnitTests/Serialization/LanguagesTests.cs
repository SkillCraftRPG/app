using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Worlds;
using SkillCraft.Infrastructure.Converters;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class LanguagesTests
{
  private readonly JsonSerializerOptions _options = new();

  public LanguagesTests()
  {
    _options.Converters.Add(new LanguageIdConverter());
    _options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    LanguageId languageId = new(WorldId.NewId());
    int extra = 1;
    string text = "Beaucoup de Haut-Elfes apprennent le <u>Harsème</u> ou le <u>Sarnique</u>.";
    string json = $@"{{""Ids"":[""{languageId}""],""Extra"":{extra},""Text"":""{text}""}}";
    Languages? languages = JsonSerializer.Deserialize<Languages>(json, _options);
    Assert.NotNull(languages);
    Assert.Equal(languageId, Assert.Single(languages.Ids));
    Assert.Equal(extra, languages.Extra);
    Assert.Equal(text, languages.Text);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    Languages? languages = JsonSerializer.Deserialize<Languages>(json, _options);
    Assert.Null(languages);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    Languages languages = new(new HashSet<LanguageId>([new LanguageId(WorldId.NewId())]), extra: 1, "Beaucoup de Haut-Elfes apprennent le <u>Harsème</u> ou le <u>Sarnique</u>.");
    string json = JsonSerializer.Serialize(languages, _options);
    Assert.Equal($@"{{""Ids"":[""{languages.Ids.Single()}""],""Extra"":{languages.Extra},""Text"":""{languages.Text}""}}", json);
  }
}
