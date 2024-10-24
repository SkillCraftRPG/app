using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Customizations;

namespace SkillCraft.Application.Customizations.Queries;

/// <exception cref="PermissionDeniedException"></exception>
public record ReadCustomizationQuery(Guid Id) : Activity, IRequest<CustomizationModel?>;

internal class ReadCustomizationQueryHandler : IRequestHandler<ReadCustomizationQuery, CustomizationModel?>
{
  private readonly ICustomizationQuerier _customizationQuerier;
  private readonly IPermissionService _permissionService;

  public ReadCustomizationQueryHandler(ICustomizationQuerier customizationQuerier, IPermissionService permissionService)
  {
    _customizationQuerier = customizationQuerier;
    _permissionService = permissionService;
  }

  public async Task<CustomizationModel?> Handle(ReadCustomizationQuery query, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanPreviewAsync(query, EntityType.Customization, cancellationToken);

    return await _customizationQuerier.ReadAsync(query.GetWorldId(), query.Id, cancellationToken);
  }
}
