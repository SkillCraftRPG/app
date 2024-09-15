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
public class UpdateCasteCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICasteQuerier> _casteQuerier = new();
  private readonly Mock<ICasteRepository> _casteRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly UpdateCasteCommandHandler _handler;

  private readonly WorldMock _world = new();

  public UpdateCasteCommandHandlerTests()
  {
    _handler = new(_casteQuerier.Object, _casteRepository.Object, _permissionService.Object, _sender.Object);
  }

  [Fact(DisplayName = "It should return null when the caste could not be found.")]
  public async Task It_should_return_null_when_the_caste_could_not_be_found()
  {
    UpdateCastePayload payload = new();
    UpdateCasteCommand command = new(Guid.Empty, payload);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateCastePayload payload = new()
    {
      WealthRoll = new Change<string>("8d6+7")
    };
    UpdateCasteCommand command = new(Guid.Empty, payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("RollValidator", error.ErrorCode);
    Assert.Equal("WealthRoll.Value", error.PropertyName);
    Assert.Equal(payload.WealthRoll.Value, error.AttemptedValue);
  }

  [Fact(DisplayName = "It should update an existing caste.")]
  public async Task It_should_update_an_existing_caste()
  {
    Caste caste = new(_world.Id, new Name("artisan"), _world.OwnerId)
    {
      Description = new Description("Peu peuvent se vanter d’avoir reçu une éducation traditionnelle comme celle du personnage. Il a suivi un parcours scolaire conforme et sans dérogation ayant mené à une instruction de haute qualité. Malgré son manque d’expériences personnelles, son grand savoir lui permet de se débrouiller même dans les situations les plus difficiles.")
    };
    caste.Update(_world.OwnerId);
    _casteRepository.Setup(x => x.LoadAsync(caste.Id, _cancellationToken)).ReturnsAsync(caste);

    UpdateCastePayload payload = new()
    {
      Name = " Artisan ",
      Description = new Change<string>("    "),
      Skill = new Change<Skill?>(Skill.Craft),
      WealthRoll = new Change<string>("8d6")
    };
    UpdateCasteCommand command = new(caste.Id.ToGuid(), payload);
    command.Contextualize();

    CasteModel model = new();
    _casteQuerier.Setup(x => x.ReadAsync(caste, _cancellationToken)).ReturnsAsync(model);

    CasteModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Key.Type == EntityType.Caste && y.Key.Id == caste.Id.ToGuid() && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveCasteCommand>(y => y.Caste.Equals(caste)
      && y.Caste.Name.Value == payload.Name.Trim()
      && y.Caste.Description == null
      && y.Caste.Skill == payload.Skill.Value
      && y.Caste.WealthRoll != null && y.Caste.WealthRoll.Value == payload.WealthRoll.Value), _cancellationToken), Times.Once);
  }
}
