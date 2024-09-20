using SkillCraft.Domain.Characters;

namespace SkillCraft.Infrastructure.Converters;

internal class PlayerNameConverter : JsonConverter<PlayerName>
{
  public override PlayerName? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return PlayerName.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, PlayerName playerName, JsonSerializerOptions options)
  {
    writer.WriteStringValue(playerName.Value);
  }
}
