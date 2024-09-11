using Logitar.Portal.Contracts.Search;
using MediatR;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Application.Worlds.Queries;

public record SearchWorldsQuery(SearchWorldsPayload Payload) : Activity, IRequest<SearchResults<WorldModel>>;

internal class SearchWorldsQueryHandler : IRequestHandler<SearchWorldsQuery, SearchResults<WorldModel>>
{
  private readonly IWorldQuerier _worldQuerier;

  public SearchWorldsQueryHandler(IWorldQuerier worldQuerier)
  {
    _worldQuerier = worldQuerier;
  }

  public async Task<SearchResults<WorldModel>> Handle(SearchWorldsQuery query, CancellationToken cancellationToken)
  {
    return await _worldQuerier.SearchAsync(query.GetUser(), query.Payload, cancellationToken);
  }
}
