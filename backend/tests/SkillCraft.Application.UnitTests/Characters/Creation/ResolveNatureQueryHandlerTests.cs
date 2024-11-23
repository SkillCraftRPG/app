using Moq;
using SkillCraft.Application.Characters.Commands;
using SkillCraft.Application.Natures;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain;
using SkillCraft.Domain.Natures;

namespace SkillCraft.Application.Characters.Creation;

[Trait(Traits.Category, Categories.Unit)]
public class ResolveNatureQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<INatureRepository> _natureRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly ResolveNatureQueryHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Nature _nature;
  private readonly CreateCharacterCommand _activity = new(Id: null, new CreateCharacterPayload());

  public ResolveNatureQueryHandlerTests()
  {
    _handler = new(_natureRepository.Object, _permissionService.Object);

    _nature = new(_world.Id, new Name("Courroucé"), _world.OwnerId);
    _natureRepository.Setup(x => x.LoadAsync(_nature.Id, _cancellationToken)).ReturnsAsync(_nature);

    _activity.Contextualize(_world);
  }

  [Fact(DisplayName = "It should return the found nature.")]
  public async Task It_should_return_the_found_nature()
  {
    ResolveNatureQuery query = new(_activity, _nature.EntityId);

    Nature nature = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_nature, nature);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(_activity, EntityType.Nature, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw NatureNotFoundException when the nature could not be found.")]
  public async Task It_should_throw_NatureNotFoundException_when_the_nature_could_not_be_found()
  {
    ResolveNatureQuery query = new(_activity, Guid.NewGuid());

    var exception = await Assert.ThrowsAsync<NatureNotFoundException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(query.Id, exception.NatureId);
    Assert.Equal("NatureId", exception.PropertyName);
  }
}
