﻿using MediatR;
using SkillCraft.Application.Customizations;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Personalities;
using SkillCraft.Domain.Worlds;
using Action = SkillCraft.Application.Permissions.Action;

namespace SkillCraft.Application.Characters.Creation;

internal record ResolveCustomizationsQuery(Activity Activity, Personality Personality, IEnumerable<Guid> Ids) : IRequest<IReadOnlyCollection<Customization>>;

internal class ResolveCustomizationsQueryHandler : IRequestHandler<ResolveCustomizationsQuery, IReadOnlyCollection<Customization>>
{
  private const string PropertyName = nameof(CreateCharacterPayload.CustomizationIds);

  private readonly ICustomizationRepository _customizationRepository;
  private readonly IPermissionService _permissionService;

  public ResolveCustomizationsQueryHandler(ICustomizationRepository customizationRepository, IPermissionService permissionService)
  {
    _customizationRepository = customizationRepository;
    _permissionService = permissionService;
  }

  public async Task<IReadOnlyCollection<Customization>> Handle(ResolveCustomizationsQuery query, CancellationToken cancellationToken)
  {
    if (!query.Ids.Any())
    {
      return [];
    }

    Activity activity = query.Activity;
    await _permissionService.EnsureCanPreviewAsync(activity, EntityType.Customization, cancellationToken);

    WorldId worldId = activity.GetWorldId();
    IEnumerable<CustomizationId> ids = query.Ids.Distinct().Select(id => new CustomizationId(worldId, id));
    IReadOnlyCollection<Customization> customizations = await _customizationRepository.LoadAsync(ids, cancellationToken);

    int gifts = 0;
    int disabilities = 0;
    foreach (Customization customization in customizations)
    {
      if (customization.WorldId != worldId)
      {
        throw new PermissionDeniedException(Action.Preview, EntityType.Customization, activity.GetUser(), activity.GetWorld(), customization.EntityId);
      }
      else if (customization.Id == query.Personality.GiftId)
      {
        throw new CustomizationsCannotIncludePersonalityGiftException(customization, PropertyName);
      }

      switch (customization.Type)
      {
        case CustomizationType.Disability:
          disabilities++;
          break;
        case CustomizationType.Gift:
          gifts++;
          break;
      }
    }
    if (gifts != disabilities)
    {
      throw new InvalidCharacterCustomizationsException(query.Ids, PropertyName);
    }

    IEnumerable<Guid> foundIds = customizations.Select(customization => customization.EntityId).Distinct();
    IEnumerable<Guid> missingIds = query.Ids.Except(foundIds).Distinct();
    if (missingIds.Any())
    {
      throw new CustomizationsNotFoundException(missingIds, PropertyName);
    }

    return customizations;
  }
}
