using SkillCraft.Domain.Talents;

namespace SkillCraft.Infrastructure.Converters;

internal class TalentIdConverter : JsonConverter<TalentId>
{
  public override TalentId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? default : new(value);
  }

  public override void Write(Utf8JsonWriter writer, TalentId talentId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(talentId.Value);
  }
}
