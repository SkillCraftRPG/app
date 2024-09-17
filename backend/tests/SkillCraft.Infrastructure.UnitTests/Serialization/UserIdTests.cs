using Logitar.EventSourcing;
using SkillCraft.Domain;
using SkillCraft.Infrastructure.Converters;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class UserIdTests
{
  private readonly JsonSerializerOptions _options = new();

  public UserIdTests()
  {
    _options.Converters.Add(new UserIdConverter());
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    string json = string.Concat('"', UserId.NewId(), '"');
    UserId userId = JsonSerializer.Deserialize<UserId>(json, _options);
    Assert.Equal(json.Trim('"'), userId.Value);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    UserId userId = JsonSerializer.Deserialize<UserId>(json, _options);
    Assert.Equal(ActorId.DefaultValue, userId.Value);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    UserId userId = UserId.NewId();
    string json = JsonSerializer.Serialize(userId, _options);
    Assert.Equal(string.Concat('"', userId, '"'), json);
  }
}
