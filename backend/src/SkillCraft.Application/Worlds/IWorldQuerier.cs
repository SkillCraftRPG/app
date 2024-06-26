﻿using SkillCraft.Contracts.Search;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds;

public interface IWorldQuerier
{
  Task<World> ReadAsync(WorldAggregate world, CancellationToken cancellationToken = default);
  Task<World?> ReadAsync(WorldId id, CancellationToken cancellationToken = default);
  Task<World?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<World?> ReadAsync(string uniqueSlug, CancellationToken cancellationToken = default);
  Task<SearchResults<World>> SearchAsync(SearchWorldsPayload payload, CancellationToken cancellationToken = default);
}
