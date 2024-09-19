using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Talents;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;

namespace SkillCraft.Application.Talents.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadTalentQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ITalentQuerier> _talentQuerier = new();

  private readonly ReadTalentQueryHandler _handler;

  private readonly TalentModel _talent = new(new WorldModel(), "Formation martiale") { Id = Guid.NewGuid() };

  public ReadTalentQueryHandlerTests()
  {
    _handler = new(_permissionService.Object, _talentQuerier.Object);

    _talentQuerier.Setup(x => x.ReadAsync(_talent.Id, _cancellationToken)).ReturnsAsync(_talent);
  }

  [Fact(DisplayName = "It should return null when no talent is found.")]
  public async Task It_should_return_null_when_no_talent_is_found()
  {
    ReadTalentQuery query = new(Guid.Empty);
    Assert.Null(await _handler.Handle(query, _cancellationToken));

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(It.IsAny<ReadTalentQuery>(), It.IsAny<EntityMetadata>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should return the talent found by ID.")]
  public async Task It_should_return_the_talent_found_by_Id()
  {
    ReadTalentQuery query = new(_talent.Id);
    TalentModel? talent = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_talent, talent);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(
      query,
      It.Is<EntityMetadata>(y => y.WorldId.ToGuid() == _talent.World.Id && y.Key.Type == EntityType.Talent && y.Key.Id == _talent.Id && y.Size > 0),
      _cancellationToken), Times.Once);
  }
}
