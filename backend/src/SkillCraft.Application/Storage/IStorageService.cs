using SkillCraft.Domain;

namespace SkillCraft.Application.Storage;

public interface IStorageService
{
  Task Async(IStoredEntity entity, CancellationToken cancellationToken = default); // TODO(fpion): rename
  Task Async(IStoredEntity entity, int previousSize, CancellationToken cancellationToken = default); // TODO(fpion): rename
}
