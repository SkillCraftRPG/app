﻿using Moq;
using SkillCraft.Application.Characters.Commands;
using SkillCraft.Application.Customizations;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Natures;

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
  private readonly Nature _nature;
  private readonly CreateCharacterCommand _activity = new(Id: null, new CreateCharacterPayload());

  public ResolveCustomizationsQueryHandlerTests()
  {
    _handler = new(_customizationRepository.Object, _permissionService.Object);

    _customization = new(_world.Id, CustomizationType.Gift, new Name("Féroce"), _world.OwnerId);
    _customizationRepository.Setup(x => x.LoadAsync(_customization.Id, _cancellationToken)).ReturnsAsync(_customization);
    _disability = new(_world.Id, CustomizationType.Disability, new Name("Pauvreté"), _world.OwnerId);
    _customizationRepository.Setup(x => x.LoadAsync(_disability.Id, _cancellationToken)).ReturnsAsync(_disability);
    _gift = new(_world.Id, CustomizationType.Gift, new Name("Réflexes"), _world.OwnerId);
    _customizationRepository.Setup(x => x.LoadAsync(_gift.Id, _cancellationToken)).ReturnsAsync(_gift);

    _nature = new(_world.Id, new Name("Courroucé"), _world.OwnerId);
    _nature.SetGift(_customization);
    _nature.Update(_world.OwnerId);

    _activity.Contextualize(_world);
  }

  [Fact(DisplayName = "It should return prematurely when there is no ID.")]
  public async Task It_should_return_prematurely_when_there_is_no_Id()
  {
    ResolveCustomizationsQuery query = new(_activity, _nature, Ids: []);

    IReadOnlyCollection<Customization> customizations = await _handler.Handle(query, _cancellationToken);
    Assert.Empty(customizations);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(It.IsAny<Activity>(), It.IsAny<EntityType>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should return the found customizations.")]
  public async Task It_should_return_the_found_customizations()
  {
    ResolveCustomizationsQuery query = new(_activity, _nature, [_disability.EntityId, _gift.EntityId]);

    IEnumerable<CustomizationId> customizationIds = query.Ids.Distinct().Select(id => new CustomizationId(_world.Id, id));
    _customizationRepository.Setup(x => x.LoadAsync(customizationIds, _cancellationToken)).ReturnsAsync([_disability, _gift]);

    IReadOnlyCollection<Customization> customizations = await _handler.Handle(query, _cancellationToken);
    Assert.Equal(2, customizations.Count);
    Assert.Contains(_disability, customizations);
    Assert.Contains(_gift, customizations);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(_activity, EntityType.Customization, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw CustomizationsCannotIncludeNatureGiftException when the nature's gift is included in customizations.")]
  public async Task It_should_throw_CustomizationsCannotIncludeNatureGiftException_when_the_nature_s_git_is_included_in_customizations()
  {
    ResolveCustomizationsQuery query = new(_activity, _nature, [_customization.EntityId, _disability.EntityId, _gift.EntityId]);

    IEnumerable<CustomizationId> customizationIds = query.Ids.Distinct().Select(id => new CustomizationId(_world.Id, id));
    _customizationRepository.Setup(x => x.LoadAsync(customizationIds, _cancellationToken)).ReturnsAsync([_customization, _disability, _gift]);

    var exception = await Assert.ThrowsAsync<CustomizationsCannotIncludeNatureGiftException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_nature.EntityId, exception.NatureId);
    Assert.Equal(_customization.EntityId, exception.GiftId);
    Assert.Equal("CustomizationIds", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw CustomizationsNotFoundException when some customizations could not be found.")]
  public async Task It_should_throw_CustomizationsNotFoundException_some_customizations_could_not_be_found()
  {
    ResolveCustomizationsQuery query = new(_activity, _nature, [_disability.EntityId, _gift.EntityId, Guid.NewGuid(), Guid.Empty]);

    IEnumerable<CustomizationId> customizationIds = query.Ids.Distinct().Select(id => new CustomizationId(_world.Id, id));
    _customizationRepository.Setup(x => x.LoadAsync(customizationIds, _cancellationToken)).ReturnsAsync([_disability, _gift]);

    var exception = await Assert.ThrowsAsync<CustomizationsNotFoundException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(query.Ids.Skip(2), exception.CustomizationIds);
    Assert.Equal("CustomizationIds", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw InvalidCharacterCustomizationsException when the number of gifts does not equal the number of disabilities.")]
  public async Task It_should_throw_InvalidCharacterCustomizationsException_when_the_number_of_gifts_does_not_equal_the_number_of_disabilities()
  {
    ResolveCustomizationsQuery query = new(_activity, _nature, [_gift.EntityId]);

    IEnumerable<CustomizationId> customizationIds = query.Ids.Distinct().Select(id => new CustomizationId(_world.Id, id));
    _customizationRepository.Setup(x => x.LoadAsync(customizationIds, _cancellationToken)).ReturnsAsync([_gift]);

    var exception = await Assert.ThrowsAsync<InvalidCharacterCustomizationsException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(query.Ids, exception.CustomizationIds);
    Assert.Equal("CustomizationIds", exception.PropertyName);
  }
}
