using FluentValidation;
using FluentValidation.Results;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Personalities;
using SkillCraft.Domain;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Personalities;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Application.Personalities.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SavePersonalityCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICustomizationRepository> _customizationRepository = new();
  private readonly Mock<IPersonalityQuerier> _personalityQuerier = new();
  private readonly Mock<IPersonalityRepository> _personalityRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly SavePersonalityCommandHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Customization _gift;
  private readonly Personality _personality;
  private readonly PersonalityModel _model = new();

  public SavePersonalityCommandHandlerTests()
  {
    _handler = new(_customizationRepository.Object, _permissionService.Object, _personalityQuerier.Object, _personalityRepository.Object, _storageService.Object);

    _gift = new(_world.Id, Contracts.Customizations.CustomizationType.Gift, new Name("Féroce"), _world.OwnerId);
    _customizationRepository.Setup(x => x.LoadAsync(_gift.Id, _cancellationToken)).ReturnsAsync(_gift);

    _personality = new(_world.Id, new Name("courrouce"), _world.OwnerId);
    _personalityRepository.Setup(x => x.LoadAsync(_personality.Id, _cancellationToken)).ReturnsAsync(_personality);

    _personalityQuerier.Setup(x => x.ReadAsync(It.IsAny<Personality>(), _cancellationToken)).ReturnsAsync(_model);
  }

  [Theory(DisplayName = "It should create a new personality.")]
  [InlineData(null)]
  [InlineData("fdf958c0-06be-4b68-a405-b4fd7b21ee23")]
  public async Task It_should_create_a_new_personality(string? idValue)
  {
    SavePersonalityPayload payload = new(" Courroucé ")
    {
      Description = "  Les émotions du personnage sont vives et ses mouvements sont brusques.  ",
      Attribute = Attribute.Agility,
      GiftId = _gift.EntityId
    };

    bool parsed = Guid.TryParse(idValue, out Guid id);
    SavePersonalityCommand command = new(parsed ? id : null, payload, Version: null);
    command.Contextualize(_world);

    SavePersonalityResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Personality);
    Assert.True(result.Created);

    _permissionService.Verify(x => x.EnsureCanCreateAsync(command, EntityType.Personality, _cancellationToken), Times.Once);

    _personalityRepository.Verify(x => x.SaveAsync(
      It.Is<Personality>(y => (!parsed || y.EntityId == id)
        && y.Name.Value == payload.Name.Trim() && y.Description != null && y.Description.Value == payload.Description.Trim()
        && y.Attribute == payload.Attribute && y.GiftId.HasValue && y.GiftId.Value.EntityId == payload.GiftId),
      _cancellationToken), Times.Once);

    VerifyStorage(parsed ? id : null);
  }

  [Fact(DisplayName = "It should replace an existing personality.")]
  public async Task It_should_replace_an_existing_personality()
  {
    SavePersonalityPayload payload = new(" Courroucé ")
    {
      Description = "  Les émotions du personnage sont vives et ses mouvements sont brusques.  ",
      Attribute = Attribute.Agility,
      GiftId = _gift.EntityId
    };

    SavePersonalityCommand command = new(_personality.EntityId, payload, Version: null);
    command.Contextualize(_world);

    SavePersonalityResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Personality);
    Assert.False(result.Created);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Personality && y.Id == _personality.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _personalityRepository.Verify(x => x.SaveAsync(
      It.Is<Personality>(y => y.Equals(_personality)
        && y.Name.Value == payload.Name.Trim() && y.Description != null && y.Description.Value == payload.Description.Trim()
        && y.Attribute == payload.Attribute && y.GiftId.HasValue && y.GiftId.Value.EntityId == payload.GiftId),
      _cancellationToken), Times.Once);

    VerifyStorage(_personality.EntityId);
  }

  [Fact(DisplayName = "It should return null when updating a personality that does not exist.")]
  public async Task It_should_return_null_when_updating_an_personality_that_does_not_exist()
  {
    SavePersonalityCommand command = new(Guid.Empty, new SavePersonalityPayload("Courroucé"), Version: 0);
    command.Contextualize(_world);

    SavePersonalityResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Null(result.Personality);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    SavePersonalityPayload payload = new();

    SavePersonalityCommand command = new(Id: null, payload, Version: null);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal("Name", error.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing personality.")]
  public async Task It_should_update_an_existing_personality()
  {
    Personality reference = new(_personality.WorldId, _personality.Name, _world.OwnerId, _personality.EntityId)
    {
      Description = _personality.Description
    };
    reference.Update(_world.OwnerId);
    _personalityRepository.Setup(x => x.LoadAsync(reference.Id, reference.Version, _cancellationToken)).ReturnsAsync(reference);

    Description description = new("Les émotions du personnage sont vives et ses mouvements sont brusques.");
    _personality.Description = description;
    _personality.Update(_world.OwnerId);

    SavePersonalityPayload payload = new(" Courroucé ")
    {
      Description = "    ",
      Attribute = Attribute.Agility,
      GiftId = _gift.EntityId
    };

    SavePersonalityCommand command = new(_personality.EntityId, payload, reference.Version);
    command.Contextualize(_world);

    SavePersonalityResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Personality);
    Assert.False(result.Created);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Personality && y.Id == _personality.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _personalityRepository.Verify(x => x.SaveAsync(
      It.Is<Personality>(y => y.Equals(_personality)
        && y.Name.Value == payload.Name.Trim() && y.Description == description
        && y.Attribute == payload.Attribute && y.GiftId.HasValue && y.GiftId.Value.EntityId == payload.GiftId),
      _cancellationToken), Times.Once);

    VerifyStorage(_personality.EntityId);
  }

  private void VerifyStorage(Guid? id)
  {
    _storageService.Verify(x => x.EnsureAvailableAsync(
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Personality && (id == null || y.Id == id) && y.Size > 0),
      _cancellationToken), Times.Once);

    _storageService.Verify(x => x.UpdateAsync(
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Personality && (id == null || y.Id == id) && y.Size > 0),
      _cancellationToken), Times.Once);
  }
}
