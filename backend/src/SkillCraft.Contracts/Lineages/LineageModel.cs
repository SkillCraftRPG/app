using Logitar.Portal.Contracts;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Contracts.Lineages;

public class LineageModel : Aggregate
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public AttributeBonusesModel Attributes { get; set; }
  public List<TraitModel> Traits { get; set; }

  public LanguagesModel Languages { get; set; }
  public NamesModel Names { get; set; }

  public SpeedsModel Speeds { get; set; }
  public SizeModel Size { get; set; }
  public WeightModel Weight { get; set; }
  public AgesModel Ages { get; set; }

  public LineageModel? Parent { get; set; }

  public WorldModel World { get; set; }

  public LineageModel() : this(new WorldModel(), string.Empty)
  {
  }

  public LineageModel(WorldModel world, string name)
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

    World = world;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
