using Logitar.Portal.Contracts;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Contracts.Lineages;

public class LineageModel : Aggregate
{
  public WorldModel World { get; set; }

  public string Name { get; set; }
  public string? Description { get; set; }

  public AttributeBonusesModel Attributes { get; set; }
  public List<FeatureModel> Features { get; set; }

  public LanguagesModel Languages { get; set; }
  public NamesModel Names { get; set; }

  public SpeedsModel Speeds { get; set; }
  public SizeModel Size { get; set; }
  public WeightModel Weight { get; set; }
  public AgesModel Ages { get; set; }

  public LineageModel? Species { get; set; }
  public List<LineageModel> Nations { get; set; }

  public LineageModel() : this(new WorldModel(), string.Empty)
  {
  }

  public LineageModel(WorldModel world, string name)
  {
    World = world;

    Name = name;

    Attributes = new();
    Features = [];

    Languages = new();
    Names = new();

    Speeds = new();
    Size = new();
    Weight = new();
    Ages = new();

    Nations = [];
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
