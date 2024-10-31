using SkillCraft.Domain.Characters;

namespace SkillCraft.EntityFrameworkCore.Entities;

internal class CharacterTalentEntity
{
  public int CharacterTalentId { get; private set; }
  public Guid Id { get; private set; }

  public CharacterEntity? Character { get; private set; }
  public int CharacterId { get; private set; }
  public TalentEntity? Talent { get; private set; }
  public int TalentId { get; private set; }

  public int Cost { get; private set; }
  public string? Precision { get; private set; }
  public string? Notes { get; private set; }

  public CharacterTalentEntity(CharacterEntity character, TalentEntity talent, Character.TalentUpdatedEvent @event)
  {
    Id = @event.RelationId;

    Character = character;
    CharacterId = character.CharacterId;
    Talent = talent;
    TalentId = talent.TalentId;

    Update(@event);
  }

  private CharacterTalentEntity()
  {
  }

  public void Update(Character.TalentUpdatedEvent @event)
  {
    Cost = @event.Talent.Cost;
    Precision = @event.Talent.Precision?.Value;
    Notes = @event.Talent.Notes?.Value;
  }

  public override bool Equals(object? obj) => obj is CharacterTalentEntity entity && entity.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{GetType()} (Id={Id})";
}
