namespace SkillCraft.Contracts.Castes;

public record SaveCastePayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public Skill? Skill { get; set; }
  public string? WealthRoll { get; set; }

  public List<TraitPayload> Traits { get; set; }

  public SaveCastePayload() : this(string.Empty)
  {
  }

  public SaveCastePayload(string name)
  {
    Name = name;

    Traits = [];
  }
}
