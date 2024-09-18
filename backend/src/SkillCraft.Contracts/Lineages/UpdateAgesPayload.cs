namespace SkillCraft.Contracts.Lineages;

public record UpdateAgesPayload
{
  public Change<int?>? Adolescent { get; set; }
  public Change<int?>? Adult { get; set; }
  public Change<int?>? Mature { get; set; }
  public Change<int?>? Venerable { get; set; }
}
