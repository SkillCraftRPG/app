﻿using Logitar.Portal.Contracts.Search;
using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Talents;
using SkillCraft.Domain;

namespace SkillCraft.Application.Talents.Queries;

public record SearchTalentsQuery(SearchTalentsPayload Payload) : Activity, IRequest<SearchResults<TalentModel>>;

internal class SearchTalentsQueryHandler : IRequestHandler<SearchTalentsQuery, SearchResults<TalentModel>>
{
  private readonly IPermissionService _permissionService;
  private readonly ITalentQuerier _talentQuerier;

  public SearchTalentsQueryHandler(IPermissionService permissionService, ITalentQuerier talentQuerier)
  {
    _permissionService = permissionService;
    _talentQuerier = talentQuerier;
  }

  public async Task<SearchResults<TalentModel>> Handle(SearchTalentsQuery query, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanPreviewAsync(query, EntityType.Talent, cancellationToken);

    return await _talentQuerier.SearchAsync(query.GetWorldId(), query.Payload, cancellationToken);
  }
}