namespace SkillCraft.Contracts.Speciez;

public record AttributesModel : IAttributes
{
  public int Agility { get; set; }
  public int Coordination { get; set; }
  public int Intellect { get; set; }
  public int Presence { get; set; }
  public int Sensitivity { get; set; }
  public int Spirit { get; set; }
  public int Vigor { get; set; }

  public int Extra { get; set; }
}
