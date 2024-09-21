using SkillCraft.Domain.Educations;

namespace SkillCraft.Infrastructure.Converters;

internal class EducationIdConverter : JsonConverter<EducationId>
{
  public override EducationId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? default : new(value);
  }

  public override void Write(Utf8JsonWriter writer, EducationId educationId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(educationId.Value);
  }
}
