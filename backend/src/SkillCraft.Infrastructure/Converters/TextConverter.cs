using SkillCraft.Domain.Comments;

namespace SkillCraft.Infrastructure.Converters;

internal class TextConverter : JsonConverter<Text>
{
  public override Text? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? null : new(value);
  }

  public override void Write(Utf8JsonWriter writer, Text text, JsonSerializerOptions options)
  {
    writer.WriteStringValue(text.Value);
  }
}
