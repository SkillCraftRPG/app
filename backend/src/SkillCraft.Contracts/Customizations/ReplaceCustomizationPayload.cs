namespace SkillCraft.Contracts.Customizations;

public record ReplaceCustomizationPayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public ReplaceCustomizationPayload() : this(string.Empty)
  {
  }

  public ReplaceCustomizationPayload(string name)
  {
    Name = name;
  }
}
