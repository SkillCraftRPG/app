using Logitar.Portal.Contracts.Actors;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Parties;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Application.Parties.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadPartyQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IPartyQuerier> _partyQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly ReadPartyQueryHandler _handler;

  private readonly UserMock _user = new();
  private readonly WorldMock _world;
  private readonly PartyModel _party;

  public ReadPartyQueryHandlerTests()
  {
    _handler = new(_partyQuerier.Object, _permissionService.Object);

    _world = new(_user);
    WorldModel worldModel = new(new Actor(_user), _world.Slug.Value)
    {
      Id = _world.Id.ToGuid()
    };
    _party = new(worldModel, "Confrérie Mystique")
    {
      Id = Guid.NewGuid()
    };
    _partyQuerier.Setup(x => x.ReadAsync(_world.Id, _party.Id, _cancellationToken)).ReturnsAsync(_party);
  }

  [Fact(DisplayName = "It should return null when no party is found.")]
  public async Task It_should_return_null_when_no_party_is_found()
  {
    ReadPartyQuery query = new(Guid.Empty);
    query.Contextualize(_world);

    Assert.Null(await _handler.Handle(query, _cancellationToken));

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Party, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return the party found by ID.")]
  public async Task It_should_return_the_party_found_by_Id()
  {
    ReadPartyQuery query = new(_party.Id);
    query.Contextualize(_world);

    PartyModel? party = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_party, party);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Party, _cancellationToken), Times.Once);
  }
}
