namespace SkillCraft.Contracts.Lineages;

public record UpdateLineagePayload
{
  public string? Name { get; set; }
  public Change<string>? Description { get; set; }

  public UpdateAttributesPayload Attributes { get; set; } = new();
  public List<UpdateTraitPayload> Traits { get; set; } = [];

  public UpdateLanguagesPayload Languages { get; set; } = new();
  public UpdateNamesPayload Names { get; set; } = new();
}
