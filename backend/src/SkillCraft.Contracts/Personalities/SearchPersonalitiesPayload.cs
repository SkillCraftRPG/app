using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Personalities;

public record SearchPersonalitiesPayload : SearchPayload
{
  public new List<PersonalitySortOption> Sort { get; set; } = [];
}
