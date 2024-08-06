using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Storage;

public record StoredEntity(WorldId WorldId, long Size);
