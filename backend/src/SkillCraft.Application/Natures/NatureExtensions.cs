using SkillCraft.Contracts;
using SkillCraft.Domain;
using SkillCraft.Domain.Natures;

namespace SkillCraft.Application.Natures;

internal static class NatureExtensions
{
  public static EntityMetadata GetMetadata(this Nature nature) => new(nature.WorldId, new EntityKey(EntityType.Nature, nature.EntityId), nature.CalculateSize());

  private static long CalculateSize(this Nature nature) => nature.Name.Size + (nature.Description?.Size ?? 0)
    + 4 /* Attribute */ + 4 /* GiftId */;
}
