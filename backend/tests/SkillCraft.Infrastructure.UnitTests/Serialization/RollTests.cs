using SkillCraft.Domain;
using SkillCraft.Infrastructure.Converters;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class RollTests
{
  private readonly JsonSerializerOptions _options = new();

  public RollTests()
  {
    _options.Converters.Add(new RollConverter());
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    string json = string.Concat('"', "4d6", '"');
    Roll? roll = JsonSerializer.Deserialize<Roll>(json, _options);
    Assert.NotNull(roll);
    Assert.Equal(json.Trim('"'), roll.Value);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    Roll? roll = JsonSerializer.Deserialize<Roll>(json, _options);
    Assert.Null(roll);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    Roll roll = new("8d8");
    string json = JsonSerializer.Serialize(roll, _options);
    Assert.Equal(string.Concat('"', roll, '"'), json);
  }
}
