namespace SkillCraft.Contracts.Lineages;

public record CreateOrReplaceLineagePayload
{
  public Guid? ParentId { get; set; }

  public string Name { get; set; }
  public string? Description { get; set; }

  public AttributeBonusesModel Attributes { get; set; }
  public List<TraitPayload> Traits { get; set; }

  public LanguagesPayload Languages { get; set; }
  public NamesModel Names { get; set; }

  public SpeedsModel Speeds { get; set; }
  public SizeModel Size { get; set; }
  public WeightModel Weight { get; set; }
  public AgesModel Ages { get; set; }

  public CreateOrReplaceLineagePayload() : this(string.Empty)
  {
  }

  public CreateOrReplaceLineagePayload(string name)
  {
    Name = name;

    Attributes = new();
    Traits = [];

    Languages = new();
    Names = new();

    Speeds = new();
    Size = new();
    Weight = new();
    Ages = new();
  }
}
