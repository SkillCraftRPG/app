using FluentValidation.Results;
using Logitar.Security.Cryptography;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Parties;
using SkillCraft.Domain;
using SkillCraft.Domain.Parties;

namespace SkillCraft.Application.Parties.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdatePartyCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IPartyQuerier> _partyQuerier = new();
  private readonly Mock<IPartyRepository> _partyRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly UpdatePartyCommandHandler _handler;

  private readonly WorldMock _world = new();

  public UpdatePartyCommandHandlerTests()
  {
    _handler = new(_partyQuerier.Object, _partyRepository.Object, _permissionService.Object, _storageService.Object);
  }

  [Fact(DisplayName = "It should return null when the party could not be found.")]
  public async Task It_should_return_null_when_the_party_could_not_be_found()
  {
    UpdatePartyPayload payload = new();
    UpdatePartyCommand command = new(Guid.Empty, payload);
    command.Contextualize(_world);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdatePartyPayload payload = new()
    {
      Name = RandomStringGenerator.GetString(Name.MaximumLength + 1)
    };
    UpdatePartyCommand command = new(Guid.Empty, payload);
    command.Contextualize(_world);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("MaximumLengthValidator", error.ErrorCode);
    Assert.Equal("Name", error.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing party.")]
  public async Task It_should_update_an_existing_party()
  {
    Party party = new(_world.Id, new Name("confrerie-mystique"), _world.OwnerId)
    {
      Description = new Description("Suivez le pèlerinage d’Ivellios et de Saviof en Orris.")
    };
    party.Update(_world.OwnerId);
    _partyRepository.Setup(x => x.LoadAsync(party.Id, _cancellationToken)).ReturnsAsync(party);

    UpdatePartyPayload payload = new()
    {
      Name = " Confrérie Mystique ",
      Description = new Change<string>("    ")
    };
    UpdatePartyCommand command = new(party.EntityId, payload);
    command.Contextualize(_world);

    PartyModel model = new();
    _partyQuerier.Setup(x => x.ReadAsync(party, _cancellationToken)).ReturnsAsync(model);

    PartyModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Party && y.Id == party.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _partyRepository.Verify(x => x.SaveAsync(
      It.Is<Party>(y => y.Equals(party) && y.Name.Value == payload.Name.Trim() && y.Description == null),
      _cancellationToken), Times.Once);

    _storageService.Verify(x => x.EnsureAvailableAsync(
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Party && y.Id == party.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);
    _storageService.Verify(x => x.UpdateAsync(
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Party && y.Id == party.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);
  }
}
