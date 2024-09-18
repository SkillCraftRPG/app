﻿using SkillCraft.Domain.Lineages;

namespace SkillCraft.Infrastructure.Converters;

internal class LineageIdConverter : JsonConverter<LineageId>
{
  public override LineageId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? default : new(value);
  }

  public override void Write(Utf8JsonWriter writer, LineageId lineageId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(lineageId.Value);
  }
}