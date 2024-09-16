namespace SkillCraft.Contracts.Personalities;

public record ReplacePersonalityPayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public Attribute? Attribute { get; set; }
  public Guid? GiftId { get; set; }

  public ReplacePersonalityPayload() : this(string.Empty)
  {
  }

  public ReplacePersonalityPayload(string name)
  {
    Name = name;
  }
}
