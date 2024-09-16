using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Customizations;

namespace SkillCraft.Application.Customizations.Queries;

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
    CustomizationModel? customization = await _customizationQuerier.ReadAsync(query.Id, cancellationToken);
    if (customization != null)
    {
      await _permissionService.EnsureCanPreviewAsync(query, customization.GetMetadata(), cancellationToken);
    }

    return customization;
  }
}
