namespace SkillCraft.Contracts.Lineages;

public record ReplaceLineagePayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public AttributesModel Attributes { get; set; }
  public List<TraitPayload> Traits { get; set; }

  public LanguagesPayload Languages { get; set; }
  public NamesModel Names { get; set; }

  public SpeedsModel Speeds { get; set; }

  public ReplaceLineagePayload() : this(string.Empty)
  {
  }

  public ReplaceLineagePayload(string name)
  {
    Name = name;

    Attributes = new();
    Traits = [];

    Languages = new();
    Names = new();

    Speeds = new();
  }
}
