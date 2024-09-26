using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Personalities;

public record SearchPersonalitiesPayload : SearchPayload
{
  public Attribute? Attribute { get; set; }
  public Guid? GiftId { get; set; }

  public new List<PersonalitySortOption> Sort { get; set; } = [];
}
