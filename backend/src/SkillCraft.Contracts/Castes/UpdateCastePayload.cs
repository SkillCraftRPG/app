namespace SkillCraft.Contracts.Castes;

public record UpdateCastePayload
{
  public string? Name { get; set; }
  public Change<string>? Description { get; set; }

  public Change<Skill?>? Skill { get; set; }
  public Change<string>? WealthRoll { get; set; }

  public List<UpdateTraitPayload> Traits { get; set; } = [];
}
