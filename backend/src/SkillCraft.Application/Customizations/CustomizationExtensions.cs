using SkillCraft.Contracts;
using SkillCraft.Domain;
using SkillCraft.Domain.Customizations;

namespace SkillCraft.Application.Customizations;

internal static class CustomizationExtensions
{
  public static EntityMetadata GetMetadata(this Customization customization) => new(customization.WorldId, new EntityKey(EntityType.Customization, customization.EntityId), customization.CalculateSize());

  private static long CalculateSize(this Customization customization) => 4 /* Type */ + customization.Name.Size + (customization.Description?.Size ?? 0);
}
