using Logitar.Portal.Contracts.Search;
using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Customizations;

namespace SkillCraft.Application.Customizations.Queries;

/// <exception cref="PermissionDeniedException"></exception>
public record SearchCustomizationsQuery(SearchCustomizationsPayload Payload) : Activity, IRequest<SearchResults<CustomizationModel>>;

internal class SearchCustomizationsQueryHandler : IRequestHandler<SearchCustomizationsQuery, SearchResults<CustomizationModel>>
{
  private readonly ICustomizationQuerier _customizationQuerier;
  private readonly IPermissionService _permissionService;

  public SearchCustomizationsQueryHandler(ICustomizationQuerier customizationQuerier, IPermissionService permissionService)
  {
    _customizationQuerier = customizationQuerier;
    _permissionService = permissionService;
  }

  public async Task<SearchResults<CustomizationModel>> Handle(SearchCustomizationsQuery query, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanPreviewAsync(query, EntityType.Customization, cancellationToken);

    return await _customizationQuerier.SearchAsync(query.GetWorldId(), query.Payload, cancellationToken);
  }
}
