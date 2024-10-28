using SkillCraft.Domain.Items;
using SkillCraft.Domain.Worlds;
using SkillCraft.Infrastructure.Converters;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class ItemIdTests
{
  private readonly JsonSerializerOptions _options = new();

  private readonly ItemId _id = new(WorldId.NewId());

  public ItemIdTests()
  {
    _options.Converters.Add(new ItemIdConverter());
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    string json = string.Concat('"', _id, '"');
    ItemId itemId = JsonSerializer.Deserialize<ItemId>(json, _options);
    Assert.Equal(json.Trim('"'), itemId.Value);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    ItemId itemId = JsonSerializer.Deserialize<ItemId>(json, _options);
    Assert.Equal(string.Empty, itemId.Value);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    string json = JsonSerializer.Serialize(_id, _options);
    Assert.Equal(string.Concat('"', _id, '"'), json);
  }
}
