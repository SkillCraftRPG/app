using FluentValidation.Results;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Educations;
using SkillCraft.Domain;
using SkillCraft.Domain.Educations;

namespace SkillCraft.Application.Educations.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class ReplaceEducationCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IEducationQuerier> _educationQuerier = new();
  private readonly Mock<IEducationRepository> _educationRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly ReplaceEducationCommandHandler _handler;

  private readonly WorldMock _world = new();

  public ReplaceEducationCommandHandlerTests()
  {
    _handler = new(_educationQuerier.Object, _educationRepository.Object, _permissionService.Object, _sender.Object);
  }

  [Fact(DisplayName = "It should replace an existing education.")]
  public async Task It_should_replace_an_existing_education()
  {
    Education education = new(_world.Id, new Name("classic"), _world.OwnerId);
    _educationRepository.Setup(x => x.LoadAsync(education.Id, _cancellationToken)).ReturnsAsync(education);

    ReplaceEducationPayload payload = new(" Classique ")
    {
      Description = "  Peu peuvent se vanter d’avoir reçu une éducation traditionnelle comme celle du personnage. Il a suivi un parcours scolaire conforme et sans dérogation ayant mené à une instruction de haute qualité. Malgré son manque d’expériences personnelles, son grand savoir lui permet de se débrouiller même dans les situations les plus difficiles.  ",
      Skill = Skill.Knowledge,
      WealthMultiplier = 12.0
    };
    ReplaceEducationCommand command = new(education.EntityId, payload, Version: null);
    command.Contextualize(_world);

    EducationModel model = new();
    _educationQuerier.Setup(x => x.ReadAsync(education, _cancellationToken)).ReturnsAsync(model);

    EducationModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Key.Type == EntityType.Education && y.Key.Id == education.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveEducationCommand>(y => y.Education.Equals(education)
      && y.Education.Name.Value == payload.Name.Trim()
      && y.Education.Description != null && y.Education.Description.Value == payload.Description.Trim()
      && y.Education.Skill == payload.Skill
      && y.Education.WealthMultiplier == payload.WealthMultiplier), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the education could not be found.")]
  public async Task It_should_return_null_when_the_education_could_not_be_found()
  {
    ReplaceEducationPayload payload = new("Classique");
    ReplaceEducationCommand command = new(Guid.Empty, payload, Version: null);
    command.Contextualize(_world);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceEducationPayload payload = new("Classique")
    {
      WealthMultiplier = -12.0
    };
    ReplaceEducationCommand command = new(Guid.Empty, payload, Version: null);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("GreaterThanValidator", error.ErrorCode);
    Assert.Equal("WealthMultiplier.Value", error.PropertyName);
    Assert.Equal(payload.WealthMultiplier, error.AttemptedValue);
  }

  [Fact(DisplayName = "It should update an existing education from a reference.")]
  public async Task It_should_update_an_existing_education_from_a_reference()
  {
    Education reference = new(_world.Id, new Name("classic"), _world.OwnerId);
    long version = reference.Version;
    _educationRepository.Setup(x => x.LoadAsync(reference.Id, version, _cancellationToken)).ReturnsAsync(reference);

    Education education = new(_world.Id, reference.Name, _world.OwnerId, reference.EntityId);
    _educationRepository.Setup(x => x.LoadAsync(education.Id, _cancellationToken)).ReturnsAsync(education);

    Skill skill = Skill.Knowledge;
    education.Skill = skill;
    education.Update(_world.OwnerId);

    ReplaceEducationPayload payload = new(" Classique ")
    {
      Description = "  Peu peuvent se vanter d’avoir reçu une éducation traditionnelle comme celle du personnage. Il a suivi un parcours scolaire conforme et sans dérogation ayant mené à une instruction de haute qualité. Malgré son manque d’expériences personnelles, son grand savoir lui permet de se débrouiller même dans les situations les plus difficiles.  ",
      Skill = null,
      WealthMultiplier = 12.0
    };
    ReplaceEducationCommand command = new(education.EntityId, payload, version);
    command.Contextualize(_world);

    EducationModel model = new();
    _educationQuerier.Setup(x => x.ReadAsync(education, _cancellationToken)).ReturnsAsync(model);

    EducationModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Education && y.Id == education.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveEducationCommand>(y => y.Education.Equals(education)
      && y.Education.Name.Value == payload.Name.Trim()
      && y.Education.Description != null && y.Education.Description.Value == payload.Description.Trim()
      && y.Education.Skill == Skill.Knowledge
      && y.Education.WealthMultiplier == payload.WealthMultiplier), _cancellationToken), Times.Once);
  }
}
