using SkillCraft.Domain;
using SkillCraft.Infrastructure.Converters;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class SlugTests
{
  private readonly JsonSerializerOptions _options = new();

  public SlugTests()
  {
    _options.Converters.Add(new SlugConverter());
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    string json = string.Concat('"', "new-world", '"');
    Slug? slug = JsonSerializer.Deserialize<Slug>(json, _options);
    Assert.NotNull(slug);
    Assert.Equal(json.Trim('"'), slug.Value);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    Slug? slug = JsonSerializer.Deserialize<Slug>(json, _options);
    Assert.Null(slug);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    Slug slug = new("new-world");
    string json = JsonSerializer.Serialize(slug, _options);
    Assert.Equal(string.Concat('"', slug, '"'), json);
  }
}
