namespace SkillCraft.Contracts.Natures;

public record CreateOrReplaceNaturePayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public Attribute? Attribute { get; set; }
  public Guid? GiftId { get; set; }

  public CreateOrReplaceNaturePayload() : this(string.Empty)
  {
  }

  public CreateOrReplaceNaturePayload(string name)
  {
    Name = name;
  }
}
