using SkillCraft.Domain.Characters;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal class CharacterLanguageEntity
{
  public CharacterEntity? Character { get; private set; }
  public int CharacterId { get; private set; }
  public LanguageEntity? Language { get; private set; }
  public int LanguageId { get; private set; }

  public string? Notes { get; private set; }

  public CharacterLanguageEntity(CharacterEntity character, LanguageEntity language, Character.LanguageUpdatedEvent @event)
  {
    Character = character;
    CharacterId = character.CharacterId;
    Language = language;
    LanguageId = language.LanguageId;

    Update(@event);
  }

  private CharacterLanguageEntity()
  {
  }

  public void Update(Character.LanguageUpdatedEvent @event)
  {
    Notes = @event.Metadata.Notes?.Value;
  }

  public override bool Equals(object? obj) => obj is CharacterLanguageEntity entity && entity.CharacterId == CharacterId && entity.LanguageId == LanguageId;
  public override int GetHashCode() => HashCode.Combine(CharacterId, LanguageId);
  public override string ToString() => $"{GetType()} (CharacterId={CharacterId}, LanguageId={LanguageId})";
}
