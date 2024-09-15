namespace SkillCraft.Contracts.Customizations;

public record CreateCustomizationPayload
{
  public CustomizationType Type { get; set; }

  public string Name { get; set; }
  public string? Description { get; set; }

  public CreateCustomizationPayload() : this(string.Empty)
  {
  }

  public CreateCustomizationPayload(string name)
  {
    Name = name;
  }
}
