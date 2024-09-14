using FluentValidation.Results;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Castes;
using SkillCraft.Domain;
using SkillCraft.Domain.Castes;

namespace SkillCraft.Application.Castes.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class ReplaceCasteCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICasteQuerier> _casteQuerier = new();
  private readonly Mock<ICasteRepository> _casteRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly ReplaceCasteCommandHandler _handler;

  private readonly WorldMock _world = new();

  public ReplaceCasteCommandHandlerTests()
  {
    _handler = new(_casteQuerier.Object, _casteRepository.Object, _permissionService.Object, _sender.Object);
  }

  [Fact(DisplayName = "It should replace an existing caste.")]
  public async Task It_should_replace_an_existing_caste()
  {
    Caste reference = new(_world.Id, new Name("artisan"), _world.OwnerId);
    long version = reference.Version;
    _casteRepository.Setup(x => x.LoadAsync(reference.Id, version, _cancellationToken)).ReturnsAsync(reference);

    Caste caste = new(_world.Id, reference.Name, _world.OwnerId, reference.Id);
    _casteRepository.Setup(x => x.LoadAsync(caste.Id, _cancellationToken)).ReturnsAsync(caste);

    Skill skill = Skill.Knowledge;
    caste.Skill = skill;
    caste.Update(_world.OwnerId);

    ReplaceCastePayload payload = new(" Artisan ");
    ReplaceCasteCommand command = new(caste.Id.ToGuid(), payload, version);
    command.Contextualize();

    CasteModel model = new();
    _casteQuerier.Setup(x => x.ReadAsync(caste, _cancellationToken)).ReturnsAsync(model);

    CasteModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Key.Type == EntityType.Caste && y.Key.Id == caste.Id.ToGuid() && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveCasteCommand>(y => y.Caste.Equals(caste)
      && y.Caste.Name.Value == payload.Name.Trim()
      && y.Caste.Skill == Skill.Knowledge), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the caste could not be found.")]
  public async Task It_should_return_null_when_the_caste_could_not_be_found()
  {
    ReplaceCastePayload payload = new("new-slug");
    ReplaceCasteCommand command = new(Guid.Empty, payload, Version: null);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceCastePayload payload = new("Artisan")
    {
      WealthRoll = "8d6+7"
    };
    ReplaceCasteCommand command = new(Guid.Empty, payload, Version: null);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("RollValidator", error.ErrorCode);
    Assert.Equal("WealthRoll", error.PropertyName);
    Assert.Equal(payload.WealthRoll, error.AttemptedValue);
  }
}
