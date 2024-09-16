using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Personalities;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;

namespace SkillCraft.Application.Personalities.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadPersonalityQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<IPersonalityQuerier> _personalityQuerier = new();

  private readonly ReadPersonalityQueryHandler _handler;

  private readonly PersonalityModel _personality = new(new WorldModel(), "Agile") { Id = Guid.NewGuid() };

  public ReadPersonalityQueryHandlerTests()
  {
    _handler = new(_permissionService.Object, _personalityQuerier.Object);

    _personalityQuerier.Setup(x => x.ReadAsync(_personality.Id, _cancellationToken)).ReturnsAsync(_personality);
  }

  [Fact(DisplayName = "It should return null when no personality is found.")]
  public async Task It_should_return_null_when_no_personality_is_found()
  {
    ReadPersonalityQuery query = new(Guid.Empty);
    Assert.Null(await _handler.Handle(query, _cancellationToken));

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(It.IsAny<ReadPersonalityQuery>(), It.IsAny<EntityMetadata>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should return the personality found by ID.")]
  public async Task It_should_return_the_personality_found_by_Id()
  {
    ReadPersonalityQuery query = new(_personality.Id);
    PersonalityModel? personality = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_personality, personality);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(
      query,
      It.Is<EntityMetadata>(y => y.WorldId.ToGuid() == _personality.World.Id && y.Key.Type == EntityType.Personality && y.Key.Id == _personality.Id && y.Size > 0),
      _cancellationToken), Times.Once);
  }
}
