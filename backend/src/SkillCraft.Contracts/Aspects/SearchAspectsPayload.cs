using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Aspects;

public record SearchAspectsPayload : SearchPayload
{
  public Attribute? Attribute { get; set; }
  public Skill? Skill { get; set; }

  public new List<AspectSortOption> Sort { get; set; } = [];
}
