using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Lineages;

public record SearchLineagesPayload : SearchPayload
{
  public Attribute? Attribute { get; set; }
  public Guid? LanguageId { get; set; }
  public Guid? ParentId { get; set; }
  public SizeCategory? SizeCategory { get; set; }

  public new List<LineageSortOption> Sort { get; set; } = [];
}
