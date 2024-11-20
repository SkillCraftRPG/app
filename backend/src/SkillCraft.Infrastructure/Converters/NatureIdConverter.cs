using SkillCraft.Domain.Natures;

namespace SkillCraft.Infrastructure.Converters;

internal class NatureIdConverter : JsonConverter<NatureId>
{
  public override NatureId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? default : new(value);
  }

  public override void Write(Utf8JsonWriter writer, NatureId natureId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(natureId.Value);
  }
}
