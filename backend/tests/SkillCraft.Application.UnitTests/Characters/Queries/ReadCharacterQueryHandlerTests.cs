using Logitar.Portal.Contracts.Actors;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Application.Characters.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadCharacterQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICharacterQuerier> _characterQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly ReadCharacterQueryHandler _handler;

  private readonly UserMock _user = new();
  private readonly WorldMock _world;
  private readonly CharacterModel _character;

  public ReadCharacterQueryHandlerTests()
  {
    _handler = new(_characterQuerier.Object, _permissionService.Object);

    _world = new(_user);
    WorldModel worldModel = new(new Actor(_user), _world.Slug.Value)
    {
      Id = _world.Id.ToGuid()
    };
    _character = new()
    {
      Id = Guid.NewGuid(),
      World = worldModel,
      Name = "Heracles Aetos"
    };
    _characterQuerier.Setup(x => x.ReadAsync(_world.Id, _character.Id, _cancellationToken)).ReturnsAsync(_character);
  }

  [Fact(DisplayName = "It should return null when no character is found.")]
  public async Task It_should_return_null_when_no_character_is_found()
  {
    ReadCharacterQuery query = new(Guid.Empty);
    query.Contextualize(_world);

    Assert.Null(await _handler.Handle(query, _cancellationToken));

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Character, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return the character found by ID.")]
  public async Task It_should_return_the_character_found_by_Id()
  {
    ReadCharacterQuery query = new(_character.Id);
    query.Contextualize(_world);

    CharacterModel? character = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_character, character);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Character, _cancellationToken), Times.Once);
  }
}
