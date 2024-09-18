using SkillCraft.Domain;
using SkillCraft.Infrastructure.Converters;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class NameTests
{
  private readonly JsonSerializerOptions _options = new();

  public NameTests()
  {
    _options.Converters.Add(new NameConverter());
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    string json = string.Concat('"', "Ungar", '"');
    Name? name = JsonSerializer.Deserialize<Name>(json, _options);
    Assert.NotNull(name);
    Assert.Equal(json.Trim('"'), name.Value);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    Name? name = JsonSerializer.Deserialize<Name>(json, _options);
    Assert.Null(name);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    Name name = new("Ungar");
    string json = JsonSerializer.Serialize(name, _options);
    Assert.Equal(string.Concat('"', name, '"'), json);
  }
}
