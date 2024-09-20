using MediatR;
using SkillCraft.Application.Customizations;
using SkillCraft.Application.Permissions;
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
    Customization? gift = null;

    if (command.Id.HasValue)
    {
      CustomizationId giftId = new(command.Id.Value);
      gift = await _customizationRepository.LoadAsync(giftId, cancellationToken)
        ?? throw new AggregateNotFoundException<Customization>(giftId.AggregateId, PropertyName);

      await _permissionService.EnsureCanPreviewAsync(command.Activity, gift.GetMetadata(), cancellationToken);

      if (gift.Type != CustomizationType.Gift)
      {
        throw new CustomizationIsNotGiftException(gift, PropertyName);
      }
    }

    command.Personality.SetGift(gift);
  }
}
