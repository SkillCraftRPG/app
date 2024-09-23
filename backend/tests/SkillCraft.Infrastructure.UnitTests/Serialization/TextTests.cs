using SkillCraft.Domain.Comments;
using SkillCraft.Infrastructure.Converters;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class TextTests
{
  private readonly JsonSerializerOptions _options = new();

  public TextTests()
  {
    _options.Converters.Add(new TextConverter());
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    string json = string.Concat('"', "Hello World!", '"');
    Text? text = JsonSerializer.Deserialize<Text>(json, _options);
    Assert.NotNull(text);
    Assert.Equal(json.Trim('"'), text.Value);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    Text? text = JsonSerializer.Deserialize<Text>(json, _options);
    Assert.Null(text);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    Text text = new("Hello World!");
    string json = JsonSerializer.Serialize(text, _options);
    Assert.Equal(string.Concat('"', text, '"'), json);
  }
}
