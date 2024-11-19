namespace SkillCraft.Contracts.Castes;

public record CreateOrReplaceCastePayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public Skill? Skill { get; set; }
  public string? WealthRoll { get; set; }

  public List<FeaturePayload> Features { get; set; }

  public CreateOrReplaceCastePayload() : this(string.Empty)
  {
  }

  public CreateOrReplaceCastePayload(string name)
  {
    Name = name;

    Features = [];
  }
}
