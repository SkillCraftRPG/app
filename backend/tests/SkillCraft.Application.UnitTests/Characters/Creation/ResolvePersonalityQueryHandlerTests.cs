using Logitar.EventSourcing;
using Moq;
using SkillCraft.Application.Characters.Commands;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain;
using SkillCraft.Domain.Personalities;

namespace SkillCraft.Application.Characters.Creation;

[Trait(Traits.Category, Categories.Unit)]
public class ResolvePersonalityQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IPersonalityRepository> _personalityRepository = new();

  private readonly ResolvePersonalityQueryHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Personality _personality;
  private readonly CreateCharacterCommand _activity = new(new CreateCharacterPayload());

  public ResolvePersonalityQueryHandlerTests()
  {
    _handler = new(_permissionService.Object, _personalityRepository.Object);

    _personality = new(_world.Id, new Name("Courroucé"), _world.OwnerId);
    _personalityRepository.Setup(x => x.LoadAsync(_personality.Id, _cancellationToken)).ReturnsAsync(_personality);

    _activity.Contextualize(_world);
  }

  [Fact(DisplayName = "It should return the found personality.")]
  public async Task It_should_return_the_found_personality()
  {
    ResolvePersonalityQuery query = new(_activity, _personality.Id.ToGuid());

    Personality personality = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_personality, personality);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(
      _activity,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Personality && y.Id == query.Id),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw AggregateNotFoundException when the personality could not be found.")]
  public async Task It_should_throw_AggregateNotFoundException_when_the_personality_could_not_be_found()
  {
    ResolvePersonalityQuery query = new(_activity, Guid.NewGuid());

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<Personality>>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(new AggregateId(query.Id).Value, exception.Id);
    Assert.Equal("PersonalityId", exception.PropertyName);
  }
}
