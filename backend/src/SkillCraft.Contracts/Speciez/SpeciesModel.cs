using Logitar.Portal.Contracts;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Contracts.Speciez;

public class SpeciesModel : Aggregate
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public AttributesModel Attributes { get; set; }
  public List<TraitModel> Traits { get; set; }

  public LanguagesModel Languages { get; set; }

  public SpeciesModel? Parent { get; set; }

  public WorldModel World { get; set; }

  public SpeciesModel() : this(new WorldModel(), string.Empty)
  {
  }

  public SpeciesModel(WorldModel world, string name)
  {
    Name = name;

    Attributes = new();
    Traits = [];

    Languages = new();

    World = world;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
