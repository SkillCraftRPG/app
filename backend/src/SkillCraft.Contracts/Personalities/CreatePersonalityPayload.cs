namespace SkillCraft.Contracts.Personalities;

public record SavePersonalityPayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public Attribute? Attribute { get; set; }
  public Guid? GiftId { get; set; }

  public SavePersonalityPayload() : this(string.Empty)
  {
  }

  public SavePersonalityPayload(string name)
  {
    Name = name;
  }
}
