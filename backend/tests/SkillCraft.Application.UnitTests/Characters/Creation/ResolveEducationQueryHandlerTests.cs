using Logitar.EventSourcing;
using Moq;
using SkillCraft.Application.Characters.Commands;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain;
using SkillCraft.Domain.Educations;

namespace SkillCraft.Application.Characters.Creation;

[Trait(Traits.Category, Categories.Unit)]
public class ResolveEducationQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IEducationRepository> _educationRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly ResolveEducationQueryHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Education _education;
  private readonly CreateCharacterCommand _activity = new(new CreateCharacterPayload());

  public ResolveEducationQueryHandlerTests()
  {
    _handler = new(_educationRepository.Object, _permissionService.Object);

    _education = new(_world.Id, new Name("Champs de bataille"), _world.OwnerId);
    _educationRepository.Setup(x => x.LoadAsync(_education.Id, _cancellationToken)).ReturnsAsync(_education);

    _activity.Contextualize(_world);
  }

  [Fact(DisplayName = "It should return the found education.")]
  public async Task It_should_return_the_found_education()
  {
    ResolveEducationQuery query = new(_activity, _education.Id.ToGuid());

    Education education = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_education, education);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(
      _activity,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Education && y.Id == query.Id),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw AggregateNotFoundException when the education could not be found.")]
  public async Task It_should_throw_AggregateNotFoundException_when_the_education_could_not_be_found()
  {
    ResolveEducationQuery query = new(_activity, Guid.NewGuid());

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<Education>>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(new AggregateId(query.Id).Value, exception.Id);
    Assert.Equal("EducationId", exception.PropertyName);
  }
}
