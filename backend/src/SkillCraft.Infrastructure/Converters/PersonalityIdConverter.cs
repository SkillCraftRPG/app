using SkillCraft.Domain.Personalities;

namespace SkillCraft.Infrastructure.Converters;

internal class PersonalityIdConverter : JsonConverter<PersonalityId>
{
  public override PersonalityId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? default : new(value);
  }

  public override void Write(Utf8JsonWriter writer, PersonalityId personalityId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(personalityId.Value);
  }
}
