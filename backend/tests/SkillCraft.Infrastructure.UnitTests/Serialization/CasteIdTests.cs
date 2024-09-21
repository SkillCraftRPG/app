using SkillCraft.Domain.Castes;
using SkillCraft.Infrastructure.Converters;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class CasteIdTests
{
  private readonly JsonSerializerOptions _options = new();

  public CasteIdTests()
  {
    _options.Converters.Add(new CasteIdConverter());
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    string json = string.Concat('"', CasteId.NewId(), '"');
    CasteId casteId = JsonSerializer.Deserialize<CasteId>(json, _options);
    Assert.Equal(json.Trim('"'), casteId.Value);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    CasteId casteId = JsonSerializer.Deserialize<CasteId>(json, _options);
    Assert.Equal(string.Empty, casteId.Value);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    CasteId casteId = CasteId.NewId();
    string json = JsonSerializer.Serialize(casteId, _options);
    Assert.Equal(string.Concat('"', casteId, '"'), json);
  }
}
