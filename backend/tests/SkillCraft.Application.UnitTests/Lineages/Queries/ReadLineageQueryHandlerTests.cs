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

  private readonly LineageModel _lineage = new(new WorldModel(), "Abruti") { Id = Guid.NewGuid() };

  public ReadLineageQueryHandlerTests()
  {
    _handler = new(_lineageQuerier.Object, _permissionService.Object);

    _lineageQuerier.Setup(x => x.ReadAsync(_lineage.Id, _cancellationToken)).ReturnsAsync(_lineage);
  }

  [Fact(DisplayName = "It should return null when no lineage is found.")]
  public async Task It_should_return_null_when_no_lineage_is_found()
  {
    ReadLineageQuery query = new(Guid.Empty);
    Assert.Null(await _handler.Handle(query, _cancellationToken));

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(It.IsAny<ReadLineageQuery>(), It.IsAny<EntityMetadata>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should return the lineage found by ID.")]
  public async Task It_should_return_the_lineage_found_by_Id()
  {
    ReadLineageQuery query = new(_lineage.Id);
    LineageModel? lineage = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_lineage, lineage);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(
      query,
      It.Is<EntityMetadata>(y => y.WorldId.ToGuid() == _lineage.World.Id && y.Key.Type == EntityType.Lineage && y.Key.Id == _lineage.Id && y.Size > 0),
      _cancellationToken), Times.Once);
  }
}
