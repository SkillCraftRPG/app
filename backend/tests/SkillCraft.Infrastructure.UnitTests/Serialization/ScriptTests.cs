using SkillCraft.Domain.Languages;
using SkillCraft.Infrastructure.Converters;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class ScriptTests
{
  private readonly JsonSerializerOptions _options = new();

  public ScriptTests()
  {
    _options.Converters.Add(new ScriptConverter());
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    string json = string.Concat('"', "Alphabet latin", '"');
    Script? script = JsonSerializer.Deserialize<Script>(json, _options);
    Assert.NotNull(script);
    Assert.Equal(json.Trim('"'), script.Value);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    Script? script = JsonSerializer.Deserialize<Script>(json, _options);
    Assert.Null(script);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    Script script = new("Alphabet latin");
    string json = JsonSerializer.Serialize(script, _options);
    Assert.Equal(string.Concat('"', script, '"'), json);
  }
}
