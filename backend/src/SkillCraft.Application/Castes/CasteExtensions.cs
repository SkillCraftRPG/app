using SkillCraft.Contracts;
using SkillCraft.Domain;
using SkillCraft.Domain.Castes;

namespace SkillCraft.Application.Castes;

internal static class CasteExtensions
{
  public static EntityMetadata GetMetadata(this Caste caste) => new(caste.WorldId, new EntityKey(EntityType.Caste, caste.EntityId), caste.CalculateSize());

  private static long CalculateSize(this Caste caste) => caste.Name.Size + (caste.Description?.Size ?? 0)
    + 4 /* Skill */ + (caste.WealthRoll?.Size ?? 0) + caste.Features.Values.Sum(feature => feature.Size);
}
