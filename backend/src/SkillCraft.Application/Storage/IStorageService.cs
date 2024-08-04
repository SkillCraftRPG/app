using SkillCraft.Domain;

namespace SkillCraft.Application.Storage;

public interface IStorageService // TODO(fpion): implement
{
  Task Async(ISizeable sizeable, CancellationToken cancellationToken); // TODO(fpion): rename, rename argument
  Task Async(ISizeable sizeable, int previousSize, CancellationToken cancellationToken); // TODO(fpion): rename, rename argument
}
