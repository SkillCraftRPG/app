using FluentValidation.Results;
using Logitar.Security.Cryptography;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Personalities;
using SkillCraft.Domain;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Personalities;

namespace SkillCraft.Application.Personalities.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdatePersonalityCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICustomizationRepository> _customizationRepository = new();
  private readonly Mock<IPersonalityQuerier> _personalityQuerier = new();
  private readonly Mock<IPersonalityRepository> _personalityRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly UpdatePersonalityCommandHandler _handler;

  private readonly WorldMock _world = new();

  public UpdatePersonalityCommandHandlerTests()
  {
    _handler = new(_customizationRepository.Object, _permissionService.Object, _personalityQuerier.Object, _personalityRepository.Object, _storageService.Object);
  }

  [Fact(DisplayName = "It should return null when the personality could not be found.")]
  public async Task It_should_return_null_when_the_personality_could_not_be_found()
  {
    UpdatePersonalityPayload payload = new();
    UpdatePersonalityCommand command = new(Guid.Empty, payload);
    command.Contextualize(_world);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdatePersonalityPayload payload = new()
    {
      Name = RandomStringGenerator.GetString(Name.MaximumLength + 1)
    };
    UpdatePersonalityCommand command = new(Guid.Empty, payload);
    command.Contextualize(_world);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("MaximumLengthValidator", error.ErrorCode);
    Assert.Equal("Name", error.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing personality.")]
  public async Task It_should_update_an_existing_personality()
  {
    Personality personality = new(_world.Id, new Name("confrerie-mystique"), _world.OwnerId)
    {
      Description = new Description("Suivez le pèlerinage d’Ivellios et de Saviof en Orris.")
    };
    personality.Update(_world.OwnerId);
    _personalityRepository.Setup(x => x.LoadAsync(personality.Id, _cancellationToken)).ReturnsAsync(personality);

    UpdatePersonalityPayload payload = new()
    {
      Name = " Confrérie Mystique ",
      Description = new Change<string>("    ")
    };
    UpdatePersonalityCommand command = new(personality.EntityId, payload);
    command.Contextualize(_world);

    PersonalityModel model = new();
    _personalityQuerier.Setup(x => x.ReadAsync(personality, _cancellationToken)).ReturnsAsync(model);

    PersonalityModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Personality && y.Id == personality.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _personalityRepository.Verify(x => x.SaveAsync(
      It.Is<Personality>(y => y.Equals(personality) && y.Name.Value == payload.Name.Trim() && y.Description == null),
      _cancellationToken), Times.Once);

    _storageService.Verify(x => x.EnsureAvailableAsync(
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Personality && y.Id == personality.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);
    _storageService.Verify(x => x.UpdateAsync(
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Personality && y.Id == personality.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);
  }
}
