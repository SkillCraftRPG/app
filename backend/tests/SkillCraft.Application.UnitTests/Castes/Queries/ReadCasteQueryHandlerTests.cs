using Logitar.Portal.Contracts.Actors;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Castes;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Application.Castes.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadCasteQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICasteQuerier> _casteQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly ReadCasteQueryHandler _handler;

  private readonly UserMock _user = new();
  private readonly WorldMock _world;
  private readonly CasteModel _caste;

  public ReadCasteQueryHandlerTests()
  {
    _handler = new(_casteQuerier.Object, _permissionService.Object);

    _world = new(_user);
    WorldModel worldModel = new(new Actor(_user), _world.Slug.Value)
    {
      Id = _world.Id.ToGuid()
    };
    _caste = new(worldModel, "Artisan")
    {
      Id = Guid.NewGuid()
    };
    _casteQuerier.Setup(x => x.ReadAsync(_world.Id, _caste.Id, _cancellationToken)).ReturnsAsync(_caste);
  }

  [Fact(DisplayName = "It should return null when no caste is found.")]
  public async Task It_should_return_null_when_no_caste_is_found()
  {
    ReadCasteQuery query = new(Guid.Empty);
    query.Contextualize(_world);

    Assert.Null(await _handler.Handle(query, _cancellationToken));

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(It.IsAny<ReadCasteQuery>(), It.IsAny<EntityMetadata>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should return the caste found by ID.")]
  public async Task It_should_return_the_caste_found_by_Id()
  {
    ReadCasteQuery query = new(_caste.Id);
    query.Contextualize(_world);

    CasteModel? caste = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_caste, caste);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Caste, _cancellationToken), Times.Once);
  }
}
