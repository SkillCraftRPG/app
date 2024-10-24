using MediatR;
using SkillCraft.Application.Customizations;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Personalities;

namespace SkillCraft.Application.Personalities.Commands;

internal record SetGiftCommand(Personality Personality, Guid? Id) : IRequest;

internal class SetGiftCommandHandler : IRequestHandler<SetGiftCommand>
{
  private const string PropertyName = nameof(Personality.GiftId);

  private readonly ICustomizationRepository _customizationRepository;

  public SetGiftCommandHandler(ICustomizationRepository customizationRepository)
  {
    _customizationRepository = customizationRepository;
  }

  public async Task Handle(SetGiftCommand command, CancellationToken cancellationToken)
  {
    Personality personality = command.Personality;

    Customization? gift = null;
    if (command.Id.HasValue)
    {
      CustomizationId giftId = new(personality.WorldId, command.Id.Value);
      gift = await _customizationRepository.LoadAsync(giftId, cancellationToken)
        ?? throw new CustomizationNotFoundException(giftId, PropertyName);
      if (gift.Type != CustomizationType.Gift)
      {
        throw new CustomizationIsNotGiftException(gift, PropertyName);
      }
    }

    personality.SetGift(gift);
  }
}
