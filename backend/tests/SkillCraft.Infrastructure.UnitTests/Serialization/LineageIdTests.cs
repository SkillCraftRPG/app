using SkillCraft.Domain.Lineages;
using SkillCraft.Infrastructure.Converters;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class LineageIdTests
{
  private readonly JsonSerializerOptions _options = new();

  public LineageIdTests()
  {
    _options.Converters.Add(new LineageIdConverter());
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    string json = string.Concat('"', LineageId.NewId(), '"');
    LineageId lineageId = JsonSerializer.Deserialize<LineageId>(json, _options);
    Assert.Equal(json.Trim('"'), lineageId.Value);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    LineageId lineageId = JsonSerializer.Deserialize<LineageId>(json, _options);
    Assert.Equal(string.Empty, lineageId.Value);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    LineageId lineageId = LineageId.NewId();
    string json = JsonSerializer.Serialize(lineageId, _options);
    Assert.Equal(string.Concat('"', lineageId, '"'), json);
  }
}
