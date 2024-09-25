using Logitar.Portal.Contracts.Search;
using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Educations;

namespace SkillCraft.Application.Educations.Queries;

public record SearchEducationsQuery(SearchEducationsPayload Payload) : Activity, IRequest<SearchResults<EducationModel>>;

internal class SearchEducationsQueryHandler : IRequestHandler<SearchEducationsQuery, SearchResults<EducationModel>>
{
  private readonly IEducationQuerier _educationQuerier;
  private readonly IPermissionService _permissionService;

  public SearchEducationsQueryHandler(IEducationQuerier educationQuerier, IPermissionService permissionService)
  {
    _educationQuerier = educationQuerier;
    _permissionService = permissionService;
  }

  public async Task<SearchResults<EducationModel>> Handle(SearchEducationsQuery query, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanPreviewAsync(query, EntityType.Education, cancellationToken);

    return await _educationQuerier.SearchAsync(query.GetWorldId(), query.Payload, cancellationToken);
  }
}
