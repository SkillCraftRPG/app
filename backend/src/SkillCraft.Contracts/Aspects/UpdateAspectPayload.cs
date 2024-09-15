namespace SkillCraft.Contracts.Aspects;

public record UpdateAspectPayload
{
  public string? Name { get; set; }
  public Change<string>? Description { get; set; }

  public AttributesModel? Attributes { get; set; }
  public SkillsModel? Skills { get; set; }
}
