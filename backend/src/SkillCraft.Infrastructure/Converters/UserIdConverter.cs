using SkillCraft.Domain;

namespace SkillCraft.Infrastructure.Converters;

internal class UserIdConverter : JsonConverter<UserId>
{
  public override UserId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? default : new(value);
  }

  public override void Write(Utf8JsonWriter writer, UserId userId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(userId.Value);
  }
}
