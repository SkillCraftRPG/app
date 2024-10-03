namespace SkillCraft.Contracts.Customizations;

public record CreateOrReplaceCustomizationPayload
{
  public CustomizationType Type { get; set; }

  public string Name { get; set; }
  public string? Description { get; set; }

  public CreateOrReplaceCustomizationPayload() : this(string.Empty)
  {
  }

  public CreateOrReplaceCustomizationPayload(string name)
  {
    Name = name;
  }
}
