using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain;

public interface IStoredEntity
{
  WorldId WorldId { get; }

  EntityType EntityType { get; }
  Guid EntityId { get; }

  int Size { get; }
}
