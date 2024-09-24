using SkillCraft.Domain;

namespace SkillCraft.Application;

internal static class EntityExtensions
{
  public static bool IsGameEntity(this EntityType type) => type != EntityType.Comment;
}
