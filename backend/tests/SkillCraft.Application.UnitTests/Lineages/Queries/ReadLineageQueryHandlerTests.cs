using Logitar.Portal.Contracts.Actors;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Application.Lineages.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadLineageQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILineageQuerier> _lineageQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly ReadLineageQueryHandler _handler;

  private readonly UserMock _user = new();
  private readonly WorldMock _world;
  private readonly LineageModel _lineage;

  public ReadLineageQueryHandlerTests()
  {
    _handler = new(_lineageQuerier.Object, _permissionService.Object);

    _world = new(_user);
    WorldModel worldModel = new(new Actor(_user), _world.Slug.Value)
    {
      Id = _world.Id.ToGuid()
    };
    _lineage = new(worldModel, "Humain")
    {
      Id = Guid.NewGuid()
    };
    _lineageQuerier.Setup(x => x.ReadAsync(_world.Id, _lineage.Id, _cancellationToken)).ReturnsAsync(_lineage);
  }

  [Fact(DisplayName = "It should return null when no lineage is found.")]
  public async Task It_should_return_null_when_no_lineage_is_found()
  {
    ReadLineageQuery query = new(Guid.Empty);
    query.Contextualize(_world);

    Assert.Null(await _handler.Handle(query, _cancellationToken));

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Lineage, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return the lineage found by ID.")]
  public async Task It_should_return_the_lineage_found_by_Id()
  {
    ReadLineageQuery query = new(_lineage.Id);
    query.Contextualize(_world);

    LineageModel? lineage = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_lineage, lineage);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Lineage, _cancellationToken), Times.Once);
  }
}
