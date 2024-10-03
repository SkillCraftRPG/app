using Moq;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Personalities;

namespace SkillCraft.Application.Personalities.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SetGiftCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICustomizationRepository> _customizationRepository = new();

  private readonly SetGiftCommandHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Customization _gift;
  private readonly Personality _personality;

  public SetGiftCommandHandlerTests()
  {
    _handler = new(_customizationRepository.Object);

    _gift = new(_world.Id, CustomizationType.Gift, new Name("Mysticisme"), _world.OwnerId);
    _customizationRepository.Setup(x => x.LoadAsync(_gift.Id, _cancellationToken)).ReturnsAsync(_gift);

    _personality = new(_world.Id, new Name("Mystérieux"), _world.OwnerId);
  }

  [Fact(DisplayName = "It should nullify the gift.")]
  public async Task It_should_nullify_the_gift()
  {
    _personality.SetGift(_gift);
    Assert.Equal(_gift.Id, _personality.GiftId);

    SetGiftCommand command = new(_personality, Id: null);
    await _handler.Handle(command, _cancellationToken);

    Assert.Null(_personality.GiftId);
  }

  [Fact(DisplayName = "It should set the gift.")]
  public async Task It_should_set_the_gift()
  {
    Assert.Null(_personality.GiftId);

    SetGiftCommand command = new(_personality, _gift.EntityId);
    await _handler.Handle(command, _cancellationToken);

    Assert.Equal(_gift.Id, _personality.GiftId);
  }

  [Fact(DisplayName = "It_should throw AggregateNotFoundException when the gift cannot be found.")]
  public async Task It_should_throw_AggregateNotFoundException_when_the_gift_cannot_be_found()
  {
    SetGiftCommand command = new(_personality, Guid.Empty);
    CustomizationId id = new(_world.Id, command.Id);

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<Customization>>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(id.Value, exception.Id);
    Assert.Equal("GiftId", exception.PropertyName);
  }
}
