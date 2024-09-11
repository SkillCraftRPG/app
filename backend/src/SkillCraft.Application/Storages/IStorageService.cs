namespace SkillCraft.Application.Storages;

public interface IStorageService
{
  Task EnsureAvailableAsync(EntityMetadata entity, CancellationToken cancellationToken = default);
  Task UpdateAsync(EntityMetadata entity, CancellationToken cancellationToken = default);
}
