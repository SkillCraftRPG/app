namespace SkillCraft.Contracts.Personalities;

public record CreatePersonalityPayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public Attribute? Attribute { get; set; }
  public Guid? GiftId { get; set; }

  public CreatePersonalityPayload() : this(string.Empty)
  {
  }

  public CreatePersonalityPayload(string name)
  {
    Name = name;
  }
}
