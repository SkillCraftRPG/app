namespace SkillCraft.Contracts.Customizations;

public record UpdateCustomizationPayload
{
  public string? Name { get; set; }
  public Change<string>? Description { get; set; }
}
