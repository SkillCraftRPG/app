﻿using Logitar.Portal.Contracts.Search;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds;

public interface IWorldQuerier
{
  Task<int> CountOwnedAsync(UserId userId, CancellationToken cancellationToken = default);

  Task<WorldId?> FindIdAsync(EntityKey entity, CancellationToken cancellationToken = default);
  Task<WorldId?> FindIdAsync(Slug slug, CancellationToken cancellationToken = default);

  Task<WorldModel> ReadAsync(World world, CancellationToken cancellationToken = default);
  Task<WorldModel?> ReadAsync(WorldId id, CancellationToken cancellationToken = default);
  Task<WorldModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<WorldModel?> ReadAsync(string slug, CancellationToken cancellationToken = default);

  Task<SearchResults<WorldModel>> SearchAsync(UserId userId, SearchWorldsPayload payload, CancellationToken cancellationToken = default);
}
