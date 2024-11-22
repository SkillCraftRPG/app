using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Characters;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal class CharacterBonusEntity
{
  public int CharacterBonusId { get; private set; }

  public Guid Id { get; private set; }

  public CharacterEntity? Character { get; private set; }
  public int CharacterId { get; private set; }

  public BonusCategory Category { get; private set; }
  public string Target { get; private set; } = string.Empty;
  public int Value { get; private set; }

  public bool IsTemporary { get; private set; }
  public string? Precision { get; private set; }
  public string? Notes { get; private set; }

  public CharacterBonusEntity(CharacterEntity character, Character.BonusUpdatedEvent @event)
  {
    Id = @event.BonusId;

    Character = character;
    CharacterId = character.CharacterId;

    Update(@event);
  }

  private CharacterBonusEntity()
  {
  }

  public void Update(Character.BonusUpdatedEvent @event)
  {
    Bonus bonus = @event.Bonus;
    Category = bonus.Category;
    Target = bonus.Target;
    Value = bonus.Value;
    IsTemporary = bonus.IsTemporary;
    Precision = bonus.Precision?.Value;
    Notes = bonus.Notes?.Value;
  }
}
