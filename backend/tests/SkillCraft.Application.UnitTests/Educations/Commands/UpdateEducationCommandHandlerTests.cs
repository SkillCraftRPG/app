using FluentValidation.Results;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Educations;
using SkillCraft.Domain;
using SkillCraft.Domain.Educations;

namespace SkillCraft.Application.Educations.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdateEducationCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IEducationQuerier> _educationQuerier = new();
  private readonly Mock<IEducationRepository> _educationRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly UpdateEducationCommandHandler _handler;

  private readonly WorldMock _world = new();

  public UpdateEducationCommandHandlerTests()
  {
    _handler = new(_educationQuerier.Object, _educationRepository.Object, _permissionService.Object, _storageService.Object);
  }

  [Fact(DisplayName = "It should return null when the education could not be found.")]
  public async Task It_should_return_null_when_the_education_could_not_be_found()
  {
    UpdateEducationPayload payload = new();
    UpdateEducationCommand command = new(Guid.Empty, payload);
    command.Contextualize(_world);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateEducationPayload payload = new()
    {
      WealthMultiplier = new Change<double?>(-12.0)
    };
    UpdateEducationCommand command = new(Guid.Empty, payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("GreaterThanValidator", error.ErrorCode);
    Assert.Equal("WealthMultiplier.Value.Value", error.PropertyName);
    Assert.Equal(payload.WealthMultiplier.Value, error.AttemptedValue);
  }

  [Fact(DisplayName = "It should update an existing education.")]
  public async Task It_should_update_an_existing_education()
  {
    Education education = new(_world.Id, new Name("classique"), _world.OwnerId)
    {
      Description = new Description("Peu peuvent se vanter d’avoir reçu une éducation traditionnelle comme celle du personnage. Il a suivi un parcours scolaire conforme et sans dérogation ayant mené à une instruction de haute qualité. Malgré son manque d’expériences personnelles, son grand savoir lui permet de se débrouiller même dans les situations les plus difficiles.")
    };
    education.Update(_world.OwnerId);
    _educationRepository.Setup(x => x.LoadAsync(education.Id, _cancellationToken)).ReturnsAsync(education);

    UpdateEducationPayload payload = new()
    {
      Name = " Classique ",
      Description = new Change<string>("    "),
      Skill = new Change<Skill?>(Skill.Knowledge),
      WealthMultiplier = new Change<double?>(12.0)
    };
    UpdateEducationCommand command = new(education.EntityId, payload);
    command.Contextualize(_world);

    EducationModel model = new();
    _educationQuerier.Setup(x => x.ReadAsync(education, _cancellationToken)).ReturnsAsync(model);

    EducationModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Education && y.Id == education.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _educationRepository.Verify(x => x.SaveAsync(
      It.Is<Education>(y => y.Equals(education) && y.Name.Value == payload.Name.Trim() && y.Description == null
        && y.Skill == payload.Skill.Value
        && y.WealthMultiplier == payload.WealthMultiplier.Value),
      _cancellationToken), Times.Once);

    _storageService.Verify(x => x.EnsureAvailableAsync(
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Education && y.Id == education.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);
    _storageService.Verify(x => x.UpdateAsync(
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Education && y.Id == education.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);
  }
}
