using SkillCraft.Contracts;
using SkillCraft.Contracts.Educations;
using SkillCraft.Domain;
using SkillCraft.Domain.Educations;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Educations;

internal static class EducationExtensions
{
  private const EntityType Type = EntityType.Education;

  public static EntityMetadata GetMetadata(this Education education)
  {
    long size = education.Name.Size + (education.Description?.Size ?? 0) + 4 + 8;
    return new EntityMetadata(education.WorldId, new EntityKey(Type, education.EntityId), size);
  }
  public static EntityMetadata GetMetadata(this EducationModel education)
  {
    long size = education.Name.Length + (education.Description?.Length ?? 0) + 4 + 8;
    return new EntityMetadata(new WorldId(education.World.Id), new EntityKey(Type, education.Id), size);
  }
}
