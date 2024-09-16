namespace SkillCraft.Contracts.Castes;

public record ReplaceCastePayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public Skill? Skill { get; set; }
  public string? WealthRoll { get; set; }

  public List<TraitPayload> Traits { get; set; }

  public ReplaceCastePayload() : this(string.Empty)
  {
  }

  public ReplaceCastePayload(string name)
  {
    Name = name;

    Traits = [];
  }
}
