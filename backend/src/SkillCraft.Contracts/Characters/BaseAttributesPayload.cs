namespace SkillCraft.Contracts.Characters;

public record BaseAttributesPayload
{
  public int Agility { get; set; }
  public int Coordination { get; set; }
  public int Intellect { get; set; }
  public int Presence { get; set; }
  public int Sensitivity { get; set; }
  public int Spirit { get; set; }
  public int Vigor { get; set; }

  public Attribute Best { get; set; }
  public Attribute Worst { get; set; }
  public List<Attribute> Optional { get; set; } = [];

  public List<Attribute> Extra { get; set; } = [];
}
