using MediatR;
using SkillCraft.Application.Customizations;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Natures;

namespace SkillCraft.Application.Natures.Commands;

internal record SetGiftCommand(Nature Nature, Guid? Id) : IRequest;

internal class SetGiftCommandHandler : IRequestHandler<SetGiftCommand>
{
  private const string PropertyName = nameof(Nature.GiftId);

  private readonly ICustomizationRepository _customizationRepository;

  public SetGiftCommandHandler(ICustomizationRepository customizationRepository)
  {
    _customizationRepository = customizationRepository;
  }

  public async Task Handle(SetGiftCommand command, CancellationToken cancellationToken)
  {
    Nature nature = command.Nature;

    Customization? gift = null;
    if (command.Id.HasValue)
    {
      CustomizationId giftId = new(nature.WorldId, command.Id.Value);
      gift = await _customizationRepository.LoadAsync(giftId, cancellationToken)
        ?? throw new CustomizationNotFoundException(giftId, PropertyName);
      if (gift.Type != CustomizationType.Gift)
      {
        throw new CustomizationIsNotGiftException(gift, PropertyName);
      }
    }

    nature.SetGift(gift);
  }
}
