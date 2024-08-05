using Logitar.Portal.Contracts.Search;
using MediatR;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Application.Worlds.Queries;

public record SearchWorldsQuery(SearchWorldsPayload Payload) : Activity, IRequest<SearchResults<World>>;
