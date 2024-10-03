using FluentValidation.Results;
using Logitar.Security.Cryptography;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Personalities;
using SkillCraft.Domain;
using SkillCraft.Domain.Personalities;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Application.Personalities.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdatePersonalityCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IPersonalityQuerier> _personalityQuerier = new();
  private readonly Mock<IPersonalityRepository> _personalityRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly UpdatePersonalityCommandHandler _handler;

  private readonly WorldMock _world = new();

  public UpdatePersonalityCommandHandlerTests()
  {
    _handler = new(_permissionService.Object, _personalityQuerier.Object, _personalityRepository.Object, _sender.Object);
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
    Personality personality = new(_world.Id, new Name("mysterieux"), _world.OwnerId)
    {
      Description = new Description("Le personnage s’entoure de secrets et de mystères, il laisse peu paraître ses idées et émotions."),
      Attribute = Attribute.Spirit
    };
    personality.Update(_world.OwnerId);
    _personalityRepository.Setup(x => x.LoadAsync(personality.Id, _cancellationToken)).ReturnsAsync(personality);

    UpdatePersonalityPayload payload = new()
    {
      Name = " Mystérieux ",
      Description = new Change<string>("    "),
      GiftId = new Change<Guid?>(Guid.NewGuid())
    };
    UpdatePersonalityCommand command = new(personality.EntityId, payload);
    command.Contextualize(_world);

    PersonalityModel model = new();
    _personalityQuerier.Setup(x => x.ReadAsync(personality, _cancellationToken)).ReturnsAsync(model);

    PersonalityModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(command, EntityType.Customization, _cancellationToken), Times.Once);
    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Personality && y.Id == personality.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(
      It.Is<SetGiftCommand>(y => y.Personality.Equals(personality) && y.Id == payload.GiftId.Value),
      _cancellationToken), Times.Once);
    _sender.Verify(x => x.Send(
      It.Is<SavePersonalityCommand>(y => y.Personality.Equals(personality)
        && y.Personality.Name.Value == payload.Name.Trim()
        && y.Personality.Description == null
        && y.Personality.Attribute == Attribute.Spirit),
      _cancellationToken), Times.Once);
  }
}
