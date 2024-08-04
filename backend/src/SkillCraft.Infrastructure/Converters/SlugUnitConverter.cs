using SkillCraft.Domain;

namespace SkillCraft.Infrastructure.Converters;

internal class SlugUnitConverter : JsonConverter<SlugUnit>
{
  public override SlugUnit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return SlugUnit.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, SlugUnit slug, JsonSerializerOptions options)
  {
    writer.WriteStringValue(slug.Value);
  }
}
