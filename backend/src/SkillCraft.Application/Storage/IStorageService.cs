using SkillCraft.Domain;

namespace SkillCraft.Application.Storage;

public interface IStorageService // TODO(fpion): implement
{
  Task Async(IStoredEntity entity, CancellationToken cancellationToken); // TODO(fpion): rename
  Task Async(IStoredEntity entity, int previousSize, CancellationToken cancellationToken); // TODO(fpion): rename
}
