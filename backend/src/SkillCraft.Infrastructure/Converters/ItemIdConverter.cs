﻿using SkillCraft.Domain.Items;

namespace SkillCraft.Infrastructure.Converters;

internal class ItemIdConverter : JsonConverter<ItemId>
{
  public override ItemId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? default : new(value);
  }

  public override void Write(Utf8JsonWriter writer, ItemId itemId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(itemId.Value);
  }
}
