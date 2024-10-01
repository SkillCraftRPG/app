using FluentValidation;
using FluentValidation.Results;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Parties;
using SkillCraft.Domain;
using SkillCraft.Domain.Parties;

namespace SkillCraft.Application.Parties.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SavePartyCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IPartyQuerier> _partyQuerier = new();
  private readonly Mock<IPartyRepository> _partyRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly SavePartyCommandHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Party _party;
  private readonly PartyModel _model = new();

  public SavePartyCommandHandlerTests()
  {
    _handler = new(_partyQuerier.Object, _partyRepository.Object, _permissionService.Object, _storageService.Object);

    _party = new(_world.Id, new Name("confrerie-mystique"), _world.OwnerId);
    _partyRepository.Setup(x => x.LoadAsync(_party.Id, _cancellationToken)).ReturnsAsync(_party);

    _partyQuerier.Setup(x => x.ReadAsync(It.IsAny<Party>(), _cancellationToken)).ReturnsAsync(_model);
  }

  [Theory(DisplayName = "It should create a new party.")]
  [InlineData(null)]
  [InlineData("aa14a97c-3d61-4aa6-93e0-3c9d7ab58e21")]
  public async Task It_should_create_a_new_party(string? idValue)
  {
    SavePartyPayload payload = new(" Confrérie Mystique ")
    {
      Description = "    "
    };

    bool parsed = Guid.TryParse(idValue, out Guid id);
    SavePartyCommand command = new(parsed ? id : null, payload, Version: null);
    command.Contextualize(_world);

    SavePartyResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Party);
    Assert.True(result.Created);

    _permissionService.Verify(x => x.EnsureCanCreateAsync(command, EntityType.Party, _cancellationToken), Times.Once);

    _partyRepository.Verify(x => x.SaveAsync(
      It.Is<Party>(y => (!parsed || y.EntityId == id)
        && y.Name.Value == payload.Name.Trim() && y.Description == null),
      _cancellationToken), Times.Once);

    VerifyStorage(parsed ? id : null);
  }

  [Fact(DisplayName = "It should replace an existing party.")]
  public async Task It_should_replace_an_existing_party()
  {
    SavePartyPayload payload = new(" Confrérie Mystique ")
    {
      Description = "    "
    };

    SavePartyCommand command = new(_party.EntityId, payload, Version: null);
    command.Contextualize(_world);

    SavePartyResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Party);
    Assert.False(result.Created);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Party && y.Id == _party.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _partyRepository.Verify(x => x.SaveAsync(
      It.Is<Party>(y => y.Equals(_party)
        && y.Name.Value == payload.Name.Trim() && y.Description == null),
      _cancellationToken), Times.Once);

    VerifyStorage(_party.EntityId);
  }

  [Fact(DisplayName = "It should return null when updating an party that does not exist.")]
  public async Task It_should_return_null_when_updating_an_party_that_does_not_exist()
  {
    SavePartyCommand command = new(Guid.Empty, new SavePartyPayload("Confrérie Mystique"), Version: 0);
    command.Contextualize(_world);

    SavePartyResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Null(result.Party);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    SavePartyPayload payload = new();

    SavePartyCommand command = new(Id: null, payload, Version: null);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal("Name", error.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing party.")]
  public async Task It_should_update_an_existing_party()
  {
    Party reference = new(_party.WorldId, _party.Name, _world.OwnerId, _party.EntityId)
    {
      Description = _party.Description
    };
    reference.Update(_world.OwnerId);
    _partyRepository.Setup(x => x.LoadAsync(reference.Id, reference.Version, _cancellationToken)).ReturnsAsync(reference);

    Description description = new("Suivez le pèlerinage d’Ivellios et de Saviof en Orris.");
    _party.Description = description;
    _party.Update(_world.OwnerId);

    SavePartyPayload payload = new(" Confrérie Mystique ")
    {
      Description = "    "
    };

    SavePartyCommand command = new(_party.EntityId, payload, reference.Version);
    command.Contextualize(_world);

    SavePartyResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Party);
    Assert.False(result.Created);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Party && y.Id == _party.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _partyRepository.Verify(x => x.SaveAsync(
      It.Is<Party>(y => y.Equals(_party)
        && y.Name.Value == payload.Name.Trim() && y.Description == description),
      _cancellationToken), Times.Once);

    VerifyStorage(_party.EntityId);
  }

  private void VerifyStorage(Guid? id)
  {
    _storageService.Verify(x => x.EnsureAvailableAsync(
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Party && (id == null || y.Id == id) && y.Size > 0),
      _cancellationToken), Times.Once);

    _storageService.Verify(x => x.UpdateAsync(
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Party && (id == null || y.Id == id) && y.Size > 0),
      _cancellationToken), Times.Once);
  }
}
