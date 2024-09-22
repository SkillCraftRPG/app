using Moq;
using SkillCraft.Application.Aspects;
using SkillCraft.Application.Characters.Commands;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain;
using SkillCraft.Domain.Aspects;
using SkillCraft.Domain.Worlds;
using Action = SkillCraft.Application.Permissions.Action;

namespace SkillCraft.Application.Characters.Creation;

[Trait(Traits.Category, Categories.Unit)]
public class ResolveAspectsQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IAspectRepository> _aspectRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly ResolveAspectsQueryHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Aspect _aspect1;
  private readonly Aspect _aspect2;
  private readonly CreateCharacterCommand _activity = new(new CreateCharacterPayload());

  public ResolveAspectsQueryHandlerTests()
  {
    _handler = new(_aspectRepository.Object, _permissionService.Object);

    _aspect1 = new(_world.Id, new Name("Farouche"), _world.OwnerId);
    _aspect2 = new(_world.Id, new Name("Farouche"), _world.OwnerId);
    _activity.Contextualize(_world);
  }

  [Fact(DisplayName = "It should return prematurely when there is no ID.")]
  public async Task It_should_return_prematurely_when_there_is_no_Id()
  {
    ResolveAspectsQuery query = new(_activity, Ids: []);

    IReadOnlyCollection<Aspect> aspects = await _handler.Handle(query, _cancellationToken);
    Assert.Empty(aspects);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(It.IsAny<Activity>(), It.IsAny<EntityType>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should return the found aspects.")]
  public async Task It_should_return_the_found_aspects()
  {
    ResolveAspectsQuery query = new(_activity, [_aspect1.Id.ToGuid(), _aspect2.Id.ToGuid()]);

    IEnumerable<AspectId> aspectIds = query.Ids.Distinct().Select(id => new AspectId(id));
    _aspectRepository.Setup(x => x.LoadAsync(aspectIds, _cancellationToken)).ReturnsAsync([_aspect1, _aspect2]);

    IReadOnlyCollection<Aspect> aspects = await _handler.Handle(query, _cancellationToken);
    Assert.Equal(2, aspects.Count);
    Assert.Contains(_aspect1, aspects);
    Assert.Contains(_aspect2, aspects);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(_activity, EntityType.Aspect, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw AspectsNotFoundException when some aspects could not be found.")]
  public async Task It_should_throw_AspectsNotFoundException_some_aspects_could_not_be_found()
  {
    ResolveAspectsQuery query = new(_activity, [_aspect1.Id.ToGuid(), Guid.NewGuid(), Guid.Empty]);

    IEnumerable<AspectId> aspectIds = query.Ids.Distinct().Select(id => new AspectId(id));
    _aspectRepository.Setup(x => x.LoadAsync(aspectIds, _cancellationToken)).ReturnsAsync([_aspect1]);

    var exception = await Assert.ThrowsAsync<AspectsNotFoundException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(query.Ids.Skip(1), exception.Ids);
    Assert.Equal("AspectIds", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException a aspect is not in the expected world.")]
  public async Task It_should_throw_PermissionDeniedException_when_a_aspect_is_not_in_the_expected_world()
  {
    Aspect aspect = new(WorldId.NewId(), new Name("Circonspect"), UserId.NewId());
    ResolveAspectsQuery query = new(_activity, [aspect.Id.ToGuid()]);

    IEnumerable<AspectId> aspectIds = query.Ids.Distinct().Select(id => new AspectId(id));
    _aspectRepository.Setup(x => x.LoadAsync(aspectIds, _cancellationToken)).ReturnsAsync([aspect]);

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(Action.Preview, exception.Action);
    Assert.Equal(EntityType.Aspect, exception.EntityType);
    Assert.Equal(_world.OwnerId.ToGuid(), exception.UserId);
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(aspect.Id.ToGuid(), exception.EntityId);
  }
}
