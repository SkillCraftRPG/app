using Moq;
using SkillCraft.Application.Characters.Commands;
using SkillCraft.Application.Customizations;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Characters;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Personalities;
using SkillCraft.Domain.Worlds;
using Action = SkillCraft.Application.Permissions.Action;

namespace SkillCraft.Application.Characters.Creation;

[Trait(Traits.Category, Categories.Unit)]
public class ResolveCustomizationsQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICustomizationRepository> _customizationRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly ResolveCustomizationsQueryHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Customization _customization;
  private readonly Customization _disability;
  private readonly Customization _gift;
  private readonly Personality _personality;
  private readonly CreateCharacterCommand _activity = new(new CreateCharacterPayload());

  public ResolveCustomizationsQueryHandlerTests()
  {
    _handler = new(_customizationRepository.Object, _permissionService.Object);

    _customization = new(_world.Id, CustomizationType.Gift, new Name("Féroce"), _world.OwnerId);
    _customizationRepository.Setup(x => x.LoadAsync(_customization.Id, _cancellationToken)).ReturnsAsync(_customization);
    _disability = new(_world.Id, CustomizationType.Disability, new Name("Pauvreté"), _world.OwnerId);
    _customizationRepository.Setup(x => x.LoadAsync(_disability.Id, _cancellationToken)).ReturnsAsync(_disability);
    _gift = new(_world.Id, CustomizationType.Gift, new Name("Réflexes"), _world.OwnerId);
    _customizationRepository.Setup(x => x.LoadAsync(_gift.Id, _cancellationToken)).ReturnsAsync(_gift);

    _personality = new(_world.Id, new Name("Courroucé"), _world.OwnerId);
    _personality.SetGift(_customization);
    _personality.Update(_world.OwnerId);

    _activity.Contextualize(_world);
  }

  [Fact(DisplayName = "It should return prematurely when there is no ID.")]
  public async Task It_should_return_prematurely_when_there_is_no_Id()
  {
    ResolveCustomizationsQuery query = new(_activity, _personality, Ids: []);

    IReadOnlyCollection<Customization> customizations = await _handler.Handle(query, _cancellationToken);
    Assert.Empty(customizations);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(It.IsAny<Activity>(), It.IsAny<EntityType>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should return the found customizations.")]
  public async Task It_should_return_the_found_customizations()
  {
    ResolveCustomizationsQuery query = new(_activity, _personality, [_disability.Id.ToGuid(), _gift.Id.ToGuid()]);

    IEnumerable<CustomizationId> customizationIds = query.Ids.Distinct().Select(id => new CustomizationId(id));
    _customizationRepository.Setup(x => x.LoadAsync(customizationIds, _cancellationToken)).ReturnsAsync([_disability, _gift]);

    IReadOnlyCollection<Customization> customizations = await _handler.Handle(query, _cancellationToken);
    Assert.Equal(2, customizations.Count);
    Assert.Contains(_disability, customizations);
    Assert.Contains(_gift, customizations);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(_activity, EntityType.Customization, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw CustomizationsCannotIncludePersonalityGiftException when the personality's gift is included in customizations.")]
  public async Task It_should_throw_CustomizationsCannotIncludePersonalityGiftException_when_the_personality_s_git_is_included_in_customizations()
  {
    ResolveCustomizationsQuery query = new(_activity, _personality, [_customization.Id.ToGuid(), _disability.Id.ToGuid(), _gift.Id.ToGuid()]);

    IEnumerable<CustomizationId> customizationIds = query.Ids.Distinct().Select(id => new CustomizationId(id));
    _customizationRepository.Setup(x => x.LoadAsync(customizationIds, _cancellationToken)).ReturnsAsync([_customization, _disability, _gift]);

    var exception = await Assert.ThrowsAsync<CustomizationsCannotIncludePersonalityGiftException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(_customization.Id.ToGuid(), exception.CustomizationId);
    Assert.Equal("CustomizationIds", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw CustomizationsNotFoundException when some customizations could not be found.")]
  public async Task It_should_throw_CustomizationsNotFoundException_some_customizations_could_not_be_found()
  {
    ResolveCustomizationsQuery query = new(_activity, _personality, [_disability.Id.ToGuid(), _gift.Id.ToGuid(), Guid.NewGuid(), Guid.Empty]);

    IEnumerable<CustomizationId> customizationIds = query.Ids.Distinct().Select(id => new CustomizationId(id));
    _customizationRepository.Setup(x => x.LoadAsync(customizationIds, _cancellationToken)).ReturnsAsync([_disability, _gift]);

    var exception = await Assert.ThrowsAsync<CustomizationsNotFoundException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(query.Ids.Skip(2), exception.Ids);
    Assert.Equal("CustomizationIds", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw InvalidCharacterCustomizationsException when the number of gifts does not equal the number of disabilities.")]
  public async Task It_should_throw_InvalidCharacterCustomizationsException_when_the_number_of_gifts_does_not_equal_the_number_of_disabilities()
  {
    ResolveCustomizationsQuery query = new(_activity, _personality, [_gift.Id.ToGuid()]);

    IEnumerable<CustomizationId> customizationIds = query.Ids.Distinct().Select(id => new CustomizationId(id));
    _customizationRepository.Setup(x => x.LoadAsync(customizationIds, _cancellationToken)).ReturnsAsync([_gift]);

    var exception = await Assert.ThrowsAsync<InvalidCharacterCustomizationsException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(query.Ids, exception.Ids);
    Assert.Equal("CustomizationIds", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException a customization is not in the expected world.")]
  public async Task It_should_throw_PermissionDeniedException_when_a_customization_is_not_in_the_expected_world()
  {
    Customization customization = new(WorldId.NewId(), CustomizationType.Gift, new Name("Réflexes"), UserId.NewId());
    ResolveCustomizationsQuery query = new(_activity, _personality, [customization.Id.ToGuid()]);

    IEnumerable<CustomizationId> customizationIds = query.Ids.Distinct().Select(id => new CustomizationId(id));
    _customizationRepository.Setup(x => x.LoadAsync(customizationIds, _cancellationToken)).ReturnsAsync([customization]);

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(Action.Preview, exception.Action);
    Assert.Equal(EntityType.Customization, exception.EntityType);
    Assert.Equal(_world.OwnerId.ToGuid(), exception.UserId);
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(customization.Id.ToGuid(), exception.EntityId);
  }
}
