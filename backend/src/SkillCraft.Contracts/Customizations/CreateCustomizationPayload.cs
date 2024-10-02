namespace SkillCraft.Contracts.Customizations;

public record SaveCustomizationPayload
{
  public CustomizationType Type { get; set; }

  public string Name { get; set; }
  public string? Description { get; set; }

  public SaveCustomizationPayload() : this(string.Empty)
  {
  }

  public SaveCustomizationPayload(string name)
  {
    Name = name;
  }
}
