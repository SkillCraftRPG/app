using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Contracts.Personalities;
using SkillCraft.Domain;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Personalities;

namespace SkillCraft.Application.Personalities.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SetGiftCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICustomizationRepository> _customizationRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly SetGiftCommandHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Customization _gift;
  private readonly Personality _personality;

  private readonly ReplacePersonalityCommand _activity;

  public SetGiftCommandHandlerTests()
  {
    _handler = new(_customizationRepository.Object, _permissionService.Object);

    _gift = new(_world.Id, CustomizationType.Gift, new Name("Mysticisme"), _world.OwnerId);
    _customizationRepository.Setup(x => x.LoadAsync(_gift.Id, _cancellationToken)).ReturnsAsync(_gift);

    _personality = new(_world.Id, new Name("Mystérieux"), _world.OwnerId);
    _personality.SetGift(_gift);
    _personality.Update(_world.OwnerId);

    _activity = new(_personality.Id.ToGuid(), new ReplacePersonalityPayload(), Version: null);
  }

  [Fact(DisplayName = "It should nullify the gift.")]
  public async Task It_should_nullify_the_gift()
  {
    SetGiftCommand command = new(_activity, _personality, Id: null);

    await _handler.Handle(command, _cancellationToken);

    Assert.Null(_personality.GiftId);
  }

  [Fact(DisplayName = "It should set the gift.")]
  public async Task It_should_set_the_gift()
  {
    Customization customization = new(_world.Id, CustomizationType.Gift, new Name("Infatigable"), _world.OwnerId);
    _customizationRepository.Setup(x => x.LoadAsync(customization.Id, _cancellationToken)).ReturnsAsync(customization);

    SetGiftCommand command = new(_activity, _personality, customization.Id.ToGuid());

    await _handler.Handle(command, _cancellationToken);

    Assert.Equal(customization.Id, _personality.GiftId);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(
      _activity,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Customization && y.Id == customization.Id.ToGuid() && y.Size > 0),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw AggregateNotFoundException when the customization could not be found.")]
  public async Task It_should_throw_AggregateNotFoundException_when_the_customization_could_not_be_found()
  {
    Customization customization = new(_world.Id, CustomizationType.Gift, new Name("Entrepôt à connaissances"), _world.OwnerId);
    SetGiftCommand command = new(_activity, _personality, customization.Id.ToGuid());

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<Customization>>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(customization.Id.Value, exception.Id);
    Assert.Equal("GiftId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw CustomizationIsNotGiftException when the customization is not a gift.")]
  public async Task It_should_throw_CustomizationIsNotGiftException_when_the_customization_is_not_a_gift()
  {
    Customization customization = new(_world.Id, CustomizationType.Disability, new Name("Maladroit"), _world.OwnerId);
    _customizationRepository.Setup(x => x.LoadAsync(customization.Id, _cancellationToken)).ReturnsAsync(customization);

    SetGiftCommand command = new(_activity, _personality, customization.Id.ToGuid());

    var exception = await Assert.ThrowsAsync<CustomizationIsNotGiftException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(customization.Id.ToGuid(), exception.CustomizationId);
    Assert.Equal("GiftId", exception.PropertyName);
  }
}
