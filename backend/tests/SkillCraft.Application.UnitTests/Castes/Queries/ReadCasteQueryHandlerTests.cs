using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Castes;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;

namespace SkillCraft.Application.Castes.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadCasteQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICasteQuerier> _casteQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly ReadCasteQueryHandler _handler;

  private readonly CasteModel _caste = new(new WorldModel(), "Classique") { Id = Guid.NewGuid() };

  public ReadCasteQueryHandlerTests()
  {
    _handler = new(_casteQuerier.Object, _permissionService.Object);

    _casteQuerier.Setup(x => x.ReadAsync(_caste.Id, _cancellationToken)).ReturnsAsync(_caste);
  }

  [Fact(DisplayName = "It should return null when no caste is found.")]
  public async Task It_should_return_null_when_no_caste_is_found()
  {
    ReadCasteQuery query = new(Guid.Empty);
    Assert.Null(await _handler.Handle(query, _cancellationToken));

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(It.IsAny<ReadCasteQuery>(), It.IsAny<EntityMetadata>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should return the caste found by ID.")]
  public async Task It_should_return_the_caste_found_by_Id()
  {
    ReadCasteQuery query = new(_caste.Id);
    CasteModel? caste = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_caste, caste);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(
      query,
      It.Is<EntityMetadata>(y => y.WorldId.ToGuid() == _caste.World.Id && y.Key.Type == EntityType.Caste && y.Key.Id == _caste.Id && y.Size > 0),
      _cancellationToken), Times.Once);
  }
}
