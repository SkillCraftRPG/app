using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Personalities;

namespace SkillCraft.Application.Personalities.Commands;

internal abstract class PersonalityCommandHandler
{
  private readonly ICustomizationRepository _customizationRepository;
  private readonly IPermissionService _permissionService;
  private readonly IPersonalityRepository _personalityRepository;
  private readonly IStorageService _storageService;

  protected PersonalityCommandHandler(
    ICustomizationRepository customizationRepository,
    IPermissionService permissionService,
    IPersonalityRepository personalityRepository,
    IStorageService storageService)
  {
    _customizationRepository = customizationRepository;
    _permissionService = permissionService;
    _personalityRepository = personalityRepository;
    _storageService = storageService;
  }

  protected async Task SaveAsync(Personality personality, CancellationToken cancellationToken)
  {
    EntityMetadata entity = personality.GetMetadata();
    await _storageService.EnsureAvailableAsync(entity, cancellationToken);

    await _personalityRepository.SaveAsync(personality, cancellationToken);

    await _storageService.UpdateAsync(entity, cancellationToken);
  }

  protected async Task SetGiftAsync(Activity activity, Personality personality, Guid? id, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanPreviewAsync(activity, EntityType.Customization, cancellationToken);

    Customization? gift = null;

    if (id.HasValue)
    {
      CustomizationId giftId = new(personality.WorldId, id.Value);
      gift = await _customizationRepository.LoadAsync(giftId, cancellationToken)
        ?? throw new AggregateNotFoundException<Customization>(giftId.AggregateId, nameof(Personality.GiftId));
    }

    personality.SetGift(gift);
  }
}
