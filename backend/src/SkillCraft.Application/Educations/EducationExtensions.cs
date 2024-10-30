using SkillCraft.Contracts;
using SkillCraft.Domain;
using SkillCraft.Domain.Educations;

namespace SkillCraft.Application.Educations;

internal static class EducationExtensions
{
  public static EntityMetadata GetMetadata(this Education education) => new(education.WorldId, new EntityKey(EntityType.Education, education.EntityId), education.CalculateSize());

  private static long CalculateSize(this Education education) => education.Name.Size + (education.Description?.Size ?? 0)
    + 4 /* Skill */ + 8 /* WealthMultiplier */;
}
