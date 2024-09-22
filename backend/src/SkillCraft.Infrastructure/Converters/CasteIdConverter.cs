﻿using SkillCraft.Domain.Castes;

namespace SkillCraft.Infrastructure.Converters;

internal class CasteIdConverter : JsonConverter<CasteId>
{
  public override CasteId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? default : new(value);
  }

  public override void Write(Utf8JsonWriter writer, CasteId casteId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(casteId.Value);
  }
}