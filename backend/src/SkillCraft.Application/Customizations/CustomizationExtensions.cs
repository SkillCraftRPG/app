using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Customizations;

internal static class CustomizationExtensions
{
  private const EntityType Type = EntityType.Customization;

  public static EntityMetadata GetMetadata(this Customization customization)
  {
    long size = 4 + customization.Name.Size + (customization.Description?.Size ?? 0);
    return new EntityMetadata(customization.WorldId, new EntityKey(Type, customization.Id.ToGuid()), size);
  }
  public static EntityMetadata GetMetadata(this CustomizationModel customization)
  {
    long size = 4 + customization.Name.Length + (customization.Description?.Length ?? 0);
    return new EntityMetadata(new WorldId(customization.World.Id), new EntityKey(Type, customization.Id), size);
  }
}
