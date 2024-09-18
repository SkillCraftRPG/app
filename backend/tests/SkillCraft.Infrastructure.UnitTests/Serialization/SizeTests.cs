using SkillCraft.Contracts;
using SkillCraft.Domain;
using SkillCraft.Domain.Lineages;
using SkillCraft.Infrastructure.Converters;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class SizeTests
{
  private readonly JsonSerializerOptions _options = new();

  public SizeTests()
  {
    _options.Converters.Add(new JsonStringEnumConverter());
    _options.Converters.Add(new RollConverter());
    _options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    SizeCategory category = SizeCategory.Medium;
    Roll roll = new("150+3d10");
    string json = $@"{{""Category"":""{category}"",""Roll"":""{roll}""}}";
    Size? size = JsonSerializer.Deserialize<Size>(json, _options);
    Assert.NotNull(size);
    Assert.Equal(category, size.Category);
    Assert.Equal(roll, size.Roll);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    Size? size = JsonSerializer.Deserialize<Size>(json, _options);
    Assert.Null(size);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    Size size = new(SizeCategory.Medium, new Roll("150+3d10"));
    string json = JsonSerializer.Serialize(size, _options);
    Assert.Equal($@"{{""Category"":""{size.Category}"",""Roll"":""{size.Roll}""}}", json);
  }
}
