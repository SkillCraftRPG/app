namespace SkillCraft.Contracts.Talents;

public record UpdateTalentPayload
{
  public string? Name { get; set; }
  public Change<string>? Description { get; set; }

  public bool? AllowMultiplePurchases { get; set; }
  public Change<Guid?>? RequiredTalentId { get; set; }
  public Change<Skill?>? Skill { get; set; }
}
