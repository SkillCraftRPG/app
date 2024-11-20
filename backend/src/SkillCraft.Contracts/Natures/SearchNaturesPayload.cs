using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Natures;

public record SearchNaturesPayload : SearchPayload
{
  public Attribute? Attribute { get; set; }
  public Guid? GiftId { get; set; }

  public new List<NatureSortOption> Sort { get; set; } = [];
}
