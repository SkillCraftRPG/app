namespace SkillCraft.Contracts.Lineages;

public record UpdateLineagePayload
{
  public string? Name { get; set; }
  public Change<string>? Description { get; set; }

  public UpdateAttributeBonusesPayload Attributes { get; set; } = new();
  public List<UpdateFeaturePayload> Features { get; set; } = [];

  public UpdateLanguagesPayload Languages { get; set; } = new();
  public UpdateNamesPayload Names { get; set; } = new();

  public UpdateSpeedsPayload Speeds { get; set; } = new();
  public UpdateSizePayload Size { get; set; } = new();
  public UpdateWeightPayload Weight { get; set; } = new();
  public UpdateAgesPayload Ages { get; set; } = new();
}
