namespace SkillCraft.Contracts.Personalities;

public record CreateOrReplacePersonalityPayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public Attribute? Attribute { get; set; }
  public Guid? GiftId { get; set; }

  public CreateOrReplacePersonalityPayload() : this(string.Empty)
  {
  }

  public CreateOrReplacePersonalityPayload(string name)
  {
    Name = name;
  }
}
