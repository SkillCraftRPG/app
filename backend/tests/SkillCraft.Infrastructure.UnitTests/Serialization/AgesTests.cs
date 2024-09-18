using SkillCraft.Domain.Lineages;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class AgesTests
{
  private readonly JsonSerializerOptions _options = new();

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    string json = @"{""Adolescent"":8,""Adult"":15,""Mature"":null,""Venerable"":35}";
    Ages? ages = JsonSerializer.Deserialize<Ages>(json, _options);
    Assert.NotNull(ages);
    Assert.Equal(8, ages.Adolescent);
    Assert.Equal(15, ages.Adult);
    Assert.Null(ages.Mature);
    Assert.Equal(35, ages.Venerable);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    Ages? ages = JsonSerializer.Deserialize<Ages>(json, _options);
    Assert.Null(ages);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    Ages ages = new(adolescent: 8, adult: 15, mature: 21, venerable: 35);
    string json = JsonSerializer.Serialize(ages, _options);
    Assert.Equal(@"{""Adolescent"":8,""Adult"":15,""Mature"":21,""Venerable"":35}", json);
  }
}
