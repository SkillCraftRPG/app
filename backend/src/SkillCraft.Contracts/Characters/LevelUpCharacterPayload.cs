namespace SkillCraft.Contracts.Characters;

public record LevelUpCharacterPayload
{
  public Attribute Attribute { get; set; }

  public LevelUpCharacterPayload() : this(default(Attribute))
  {
  }

  public LevelUpCharacterPayload(Attribute attribute)
  {
    Attribute = attribute;
  }
}
