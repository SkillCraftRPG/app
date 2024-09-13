using SkillCraft.Contracts.Castes;
using SkillCraft.Domain;
using SkillCraft.Domain.Castes;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Castes;

internal static class CasteExtensions
{
  private const EntityType Type = EntityType.Caste;

  public static EntityMetadata GetMetadata(this Caste caste)
  {
    long size = caste.Name.Size + (caste.Description?.Size ?? 0) + 4 + (caste.WealthRoll?.Size ?? 0); // TODO(fpion): Traits
    return new EntityMetadata(caste.WorldId, new EntityKey(Type, caste.Id.ToGuid()), size);
  }
  public static EntityMetadata GetMetadata(this CasteModel caste)
  {
    long size = caste.Name.Length + (caste.Description?.Length ?? 0) + 4 + (caste.WealthRoll?.Length ?? 0); // TODO(fpion): Traits
    return new EntityMetadata(new WorldId(caste.World.Id), new EntityKey(Type, caste.Id), size);
  }
}
