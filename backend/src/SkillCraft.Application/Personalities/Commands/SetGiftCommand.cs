using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Personalities;

namespace SkillCraft.Application.Personalities.Commands;

internal record SetGiftCommand(Activity Activity, Personality Personality, Guid? Id) : IRequest;

internal class SetGiftCommandHandler : IRequestHandler<SetGiftCommand>
{
  private const string PropertyName = nameof(Personality.GiftId);

  private readonly ICustomizationRepository _customizationRepository;
  private readonly IPermissionService _permissionService;

  public SetGiftCommandHandler(ICustomizationRepository customizationRepository, IPermissionService permissionService)
  {
    _customizationRepository = customizationRepository;
    _permissionService = permissionService;
  }

  public async Task Handle(SetGiftCommand command, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanPreviewAsync(command.Activity, EntityType.Customization, cancellationToken);

    Personality personality = command.Personality;
    Customization? gift = null;

    if (command.Id.HasValue)
    {
      CustomizationId giftId = new(personality.WorldId, command.Id.Value);
      gift = await _customizationRepository.LoadAsync(giftId, cancellationToken)
        ?? throw new AggregateNotFoundException<Customization>(giftId.AggregateId, PropertyName);

      if (gift.Type != CustomizationType.Gift)
      {
        throw new CustomizationIsNotGiftException(gift, PropertyName);
      }
    }

    personality.SetGift(gift);
  }
}
