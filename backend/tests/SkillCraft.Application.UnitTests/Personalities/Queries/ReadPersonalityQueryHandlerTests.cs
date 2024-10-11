using Logitar.Portal.Contracts.Actors;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Personalities;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Application.Personalities.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadPersonalityQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IPersonalityQuerier> _personalityQuerier = new();

  private readonly ReadPersonalityQueryHandler _handler;

  private readonly UserMock _user = new();
  private readonly WorldMock _world;
  private readonly PersonalityModel _personality;

  public ReadPersonalityQueryHandlerTests()
  {
    _handler = new(_permissionService.Object, _personalityQuerier.Object);

    _world = new(_user);
    WorldModel worldModel = new(new Actor(_user), _world.Slug.Value)
    {
      Id = _world.Id.ToGuid()
    };
    _personality = new(worldModel, "Agile")
    {
      Id = Guid.NewGuid()
    };
    _personalityQuerier.Setup(x => x.ReadAsync(_world.Id, _personality.Id, _cancellationToken)).ReturnsAsync(_personality);
  }

  [Fact(DisplayName = "It should return null when no personality is found.")]
  public async Task It_should_return_null_when_no_personality_is_found()
  {
    ReadPersonalityQuery query = new(Guid.Empty);
    query.Contextualize(_world);

    Assert.Null(await _handler.Handle(query, _cancellationToken));

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Personality, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return the personality found by ID.")]
  public async Task It_should_return_the_personality_found_by_Id()
  {
    ReadPersonalityQuery query = new(_personality.Id);
    query.Contextualize(_world);

    PersonalityModel? personality = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_personality, personality);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Personality, _cancellationToken), Times.Once);
  }
}
