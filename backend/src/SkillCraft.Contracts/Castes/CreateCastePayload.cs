namespace SkillCraft.Contracts.Castes;

public record CreateCastePayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public Skill? Skill { get; set; }
  public string? WealthRoll { get; set; }

  public List<TraitPayload> Traits { get; set; }

  public CreateCastePayload() : this(string.Empty)
  {
  }

  public CreateCastePayload(string name)
  {
    Name = name;

    Traits = [];
  }
}
