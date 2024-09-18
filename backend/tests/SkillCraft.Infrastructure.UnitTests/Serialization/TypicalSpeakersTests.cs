using SkillCraft.Domain.Languages;
using SkillCraft.Infrastructure.Converters;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class TypicalSpeakersTests
{
  private readonly JsonSerializerOptions _options = new();

  public TypicalSpeakersTests()
  {
    _options.Converters.Add(new TypicalSpeakersConverter());
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    string json = string.Concat('"', "Humains", '"');
    TypicalSpeakers? typicalspeakers = JsonSerializer.Deserialize<TypicalSpeakers>(json, _options);
    Assert.NotNull(typicalspeakers);
    Assert.Equal(json.Trim('"'), typicalspeakers.Value);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    TypicalSpeakers? typicalspeakers = JsonSerializer.Deserialize<TypicalSpeakers>(json, _options);
    Assert.Null(typicalspeakers);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    TypicalSpeakers typicalspeakers = new("Humains");
    string json = JsonSerializer.Serialize(typicalspeakers, _options);
    Assert.Equal(string.Concat('"', typicalspeakers, '"'), json);
  }
}
