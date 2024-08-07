using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;

namespace SkillCraft.Contracts.Worlds;

public class World : Aggregate
{
  public string UniqueSlug { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public Actor Owner { get; set; }

  public World() : this(new Actor(), string.Empty)
  {
  }

  public World(Actor owner, string uniqueSlug)
  {
    UniqueSlug = uniqueSlug;

    Owner = owner;
  }

  public override string ToString() => $"{DisplayName ?? UniqueSlug} | {base.ToString()}";
}
