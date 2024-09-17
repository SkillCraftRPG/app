using SkillCraft.Domain;
using SkillCraft.Infrastructure.Converters;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class DescriptionTests
{
  private readonly JsonSerializerOptions _options = new();

  public DescriptionTests()
  {
    _options.Converters.Add(new DescriptionConverter());
    _options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    string json = string.Concat('"', "Ceci est le monde d’Ungar.", '"');
    Description? description = JsonSerializer.Deserialize<Description>(json, _options);
    Assert.NotNull(description);
    Assert.Equal(json.Trim('"'), description.Value);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    Description? description = JsonSerializer.Deserialize<Description>(json, _options);
    Assert.Null(description);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    Description description = new("Ceci est le monde d’Ungar.");
    string json = JsonSerializer.Serialize(description, _options);
    Assert.Equal(string.Concat('"', description, '"'), json);
  }
}
