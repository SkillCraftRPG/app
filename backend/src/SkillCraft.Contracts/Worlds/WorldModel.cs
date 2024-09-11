using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;

namespace SkillCraft.Contracts.Worlds;

public class WorldModel : Aggregate
{
  public string Slug { get; set; }
  public string? Name { get; set; }
  public string? Description { get; set; }

  public Actor Owner { get; set; }

  public WorldModel() : this(new Actor(), string.Empty)
  {
  }

  public WorldModel(Actor owner, string slug)
  {
    Owner = owner;

    Slug = slug;
  }

  public override string ToString() => $"{Name ?? Slug} | {base.ToString()}";
}
