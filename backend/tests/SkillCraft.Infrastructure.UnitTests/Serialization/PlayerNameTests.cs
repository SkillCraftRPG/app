using Bogus;
using SkillCraft.Domain.Characters;
using SkillCraft.Infrastructure.Converters;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class PlayerNameTests
{
  private readonly Faker _faker = new();
  private readonly JsonSerializerOptions _options = new();

  public PlayerNameTests()
  {
    _options.Converters.Add(new PlayerNameConverter());
    _options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    string json = string.Concat('"', _faker.Person.FullName, '"');
    PlayerName? name = JsonSerializer.Deserialize<PlayerName>(json, _options);
    Assert.NotNull(name);
    Assert.Equal(json.Trim('"'), name.Value);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    PlayerName? name = JsonSerializer.Deserialize<PlayerName>(json, _options);
    Assert.Null(name);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    PlayerName name = new(_faker.Person.FullName);
    string json = JsonSerializer.Serialize(name, _options);
    Assert.Equal(string.Concat('"', name, '"'), json);
  }
}
