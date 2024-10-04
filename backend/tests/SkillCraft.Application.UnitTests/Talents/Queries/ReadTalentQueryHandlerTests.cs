using Logitar.Portal.Contracts.Actors;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Talents;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Application.Talents.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadTalentQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ITalentQuerier> _talentQuerier = new();

  private readonly ReadTalentQueryHandler _handler;

  private readonly UserMock _user = new();
  private readonly WorldMock _world;
  private readonly TalentModel _talent;

  public ReadTalentQueryHandlerTests()
  {
    _handler = new(_permissionService.Object, _talentQuerier.Object);

    _world = new(_user);
    WorldModel worldModel = new(new Actor(_user), _world.Slug.Value)
    {
      Id = _world.Id.ToGuid()
    };
    _talent = new(worldModel, "Formation martiale")
    {
      Id = Guid.NewGuid()
    };
    _talentQuerier.Setup(x => x.ReadAsync(_world.Id, _talent.Id, _cancellationToken)).ReturnsAsync(_talent);
  }

  [Fact(DisplayName = "It should return null when no talent is found.")]
  public async Task It_should_return_null_when_no_talent_is_found()
  {
    ReadTalentQuery query = new(Guid.Empty);
    query.Contextualize(_world);

    Assert.Null(await _handler.Handle(query, _cancellationToken));

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Talent, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return the talent found by ID.")]
  public async Task It_should_return_the_talent_found_by_Id()
  {
    ReadTalentQuery query = new(_talent.Id);
    query.Contextualize(_world);

    TalentModel? talent = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_talent, talent);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Talent, _cancellationToken), Times.Once);
  }
}
