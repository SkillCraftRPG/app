using FluentValidation;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Educations;
using SkillCraft.Domain;
using SkillCraft.Domain.Educations;

namespace SkillCraft.Application.Educations.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveEducationCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IEducationQuerier> _educationQuerier = new();
  private readonly Mock<IEducationRepository> _educationRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IStorageService> _storageService = new();

  private readonly SaveEducationCommandHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Education _education;
  private readonly EducationModel _model = new();

  public SaveEducationCommandHandlerTests()
  {
    _handler = new(_educationQuerier.Object, _educationRepository.Object, _permissionService.Object, _storageService.Object);

    _education = new(_world.Id, new Name("classique"), _world.OwnerId);
    _educationRepository.Setup(x => x.LoadAsync(_education.Id, _cancellationToken)).ReturnsAsync(_education);

    _educationQuerier.Setup(x => x.ReadAsync(It.IsAny<Education>(), _cancellationToken)).ReturnsAsync(_model);
  }

  [Theory(DisplayName = "It should create a new education.")]
  [InlineData(null)]
  [InlineData("e18fe75c-7edf-4bff-92ba-2e91c5eae62c")]
  public async Task It_should_create_a_new_education(string? idValue)
  {
    SaveEducationPayload payload = new(" Classique ")
    {
      Description = "    ",
      Skill = Skill.Knowledge,
      WealthMultiplier = 12.0
    };

    bool parsed = Guid.TryParse(idValue, out Guid id);
    SaveEducationCommand command = new(parsed ? id : null, payload, Version: null);
    command.Contextualize(_world);

    SaveEducationResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Education);
    Assert.True(result.Created);

    _permissionService.Verify(x => x.EnsureCanCreateAsync(command, EntityType.Education, _cancellationToken), Times.Once);

    _educationRepository.Verify(x => x.SaveAsync(
      It.Is<Education>(y => (!parsed || y.EntityId == id)
        && y.Name.Value == payload.Name.Trim() && y.Description == null
        && y.Skill == payload.Skill && y.WealthMultiplier == payload.WealthMultiplier),
      _cancellationToken), Times.Once);

    VerifyStorage(parsed ? id : null);
  }

  [Fact(DisplayName = "It should replace an existing education.")]
  public async Task It_should_replace_an_existing_education()
  {
    SaveEducationPayload payload = new(" Classique ")
    {
      Description = "    ",
      Skill = Skill.Knowledge,
      WealthMultiplier = 12.0
    };

    SaveEducationCommand command = new(_education.EntityId, payload, Version: null);
    command.Contextualize(_world);

    SaveEducationResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Education);
    Assert.False(result.Created);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Education && y.Id == _education.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _educationRepository.Verify(x => x.SaveAsync(
      It.Is<Education>(y => y.Equals(_education)
        && y.Name.Value == payload.Name.Trim() && y.Description == null
        && y.Skill == payload.Skill && y.WealthMultiplier == payload.WealthMultiplier),
      _cancellationToken), Times.Once);

    VerifyStorage(_education.EntityId);
  }

  [Fact(DisplayName = "It should return null when updating an education that does not exist.")]
  public async Task It_should_return_null_when_updating_an_education_that_does_not_exist()
  {
    SaveEducationCommand command = new(Guid.Empty, new SaveEducationPayload("Artisan"), Version: 0);
    command.Contextualize(_world);

    SaveEducationResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Null(result.Education);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    SaveEducationPayload payload = new()
    {
      Skill = (Skill)(-1)
    };

    SaveEducationCommand command = new(Id: null, payload, Version: null);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Name");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "EnumValidator" && e.PropertyName == "Skill.Value");
  }

  [Fact(DisplayName = "It should update an existing education.")]
  public async Task It_should_update_an_existing_education()
  {
    Education reference = new(_education.WorldId, _education.Name, _world.OwnerId, _education.EntityId)
    {
      Description = _education.Description,
      Skill = _education.Skill,
      WealthMultiplier = _education.WealthMultiplier
    };
    reference.Update(_world.OwnerId);
    _educationRepository.Setup(x => x.LoadAsync(reference.Id, reference.Version, _cancellationToken)).ReturnsAsync(reference);

    Description description = new("Peu peuvent se vanter d’avoir reçu une éducation traditionnelle comme celle du personnage. Il a suivi un parcours scolaire conforme et sans dérogation ayant mené à une instruction de haute qualité. Malgré son manque d’expériences personnelles, son grand savoir lui permet de se débrouiller même dans les situations les plus difficiles.");
    _education.Description = description;
    _education.Update(_world.OwnerId);

    SaveEducationPayload payload = new(" Classique ")
    {
      Description = "    ",
      Skill = Skill.Knowledge,
      WealthMultiplier = 12.0
    };

    SaveEducationCommand command = new(_education.EntityId, payload, reference.Version);
    command.Contextualize(_world);

    SaveEducationResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Education);
    Assert.False(result.Created);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Education && y.Id == _education.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _educationRepository.Verify(x => x.SaveAsync(
      It.Is<Education>(y => y.Equals(_education)
        && y.Name.Value == payload.Name.Trim() && y.Description == description
        && y.Skill == payload.Skill && y.WealthMultiplier == payload.WealthMultiplier),
      _cancellationToken), Times.Once);

    VerifyStorage(_education.EntityId);
  }

  private void VerifyStorage(Guid? id)
  {
    _storageService.Verify(x => x.EnsureAvailableAsync(
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Education && (id == null || y.Id == id) && y.Size > 0),
      _cancellationToken), Times.Once);

    _storageService.Verify(x => x.UpdateAsync(
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Education && (id == null || y.Id == id) && y.Size > 0),
      _cancellationToken), Times.Once);
  }
}
