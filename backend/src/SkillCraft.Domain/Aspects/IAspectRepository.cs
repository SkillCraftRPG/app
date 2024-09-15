﻿namespace SkillCraft.Domain.Aspects;

public interface IAspectRepository
{
  Task<IReadOnlyCollection<Aspect>> LoadAsync(CancellationToken cancellationToken = default);

  Task<Aspect?> LoadAsync(AspectId id, CancellationToken cancellationToken = default);
  Task<Aspect?> LoadAsync(AspectId id, long? version, CancellationToken cancellationToken = default);

  Task SaveAsync(Aspect aspect, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Aspect> aspects, CancellationToken cancellationToken = default);
}