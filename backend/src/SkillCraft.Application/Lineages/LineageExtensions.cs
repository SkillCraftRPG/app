using SkillCraft.Contracts;
using SkillCraft.Domain;
using SkillCraft.Domain.Lineages;

namespace SkillCraft.Application.Lineages;

internal static class LineageExtensions
{
  public static EntityMetadata GetMetadata(this Lineage lineage) => new(lineage.WorldId, new EntityKey(EntityType.Lineage, lineage.EntityId), lineage.CalculateSize());

  private static long CalculateSize(this Lineage lineage) => 4 /* ParentId */ + lineage.Name.Size + (lineage.Description?.Size ?? 0)
    + 32 /* Attributes */ + lineage.Traits.Values.Sum(trait => trait.Size)
    + lineage.Languages.Size + lineage.Names.Size
    + 24 /* Speeds */ + lineage.Size.GetSize() + lineage.Weight.Size + 16 /* Ages */;
}
