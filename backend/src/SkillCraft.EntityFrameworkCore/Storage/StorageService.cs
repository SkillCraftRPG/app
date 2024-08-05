using SkillCraft.Application.Storage;
using SkillCraft.Domain;

namespace SkillCraft.EntityFrameworkCore.Storage;

internal class StorageService : IStorageService
{
  public async Task Async(IStoredEntity entity, CancellationToken cancellationToken)
  {
    await Async(entity, previousSize: 0, cancellationToken);
  }
  public async Task Async(IStoredEntity entity, int previousSize, CancellationToken cancellationToken)
  {
    int sizeDelta = entity.Size - previousSize;
    if (sizeDelta > 0)
    {
      await Task.Delay(1, cancellationToken);
      throw new NotImplementedException(); // TODO(fpion): implement
    }
  }
}
