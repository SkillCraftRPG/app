using Moq;
using SkillCraft.Application.Characters.Commands;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain;
using SkillCraft.Domain.Castes;

namespace SkillCraft.Application.Characters.Creation;

[Trait(Traits.Category, Categories.Unit)]
public class ResolveCasteQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICasteRepository> _casteRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly ResolveCasteQueryHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Caste _caste;
  private readonly CreateCharacterCommand _activity = new(new CreateCharacterPayload());

  public ResolveCasteQueryHandlerTests()
  {
    _handler = new(_casteRepository.Object, _permissionService.Object);

    _caste = new(_world.Id, new Name("Milicien"), _world.OwnerId);
    _casteRepository.Setup(x => x.LoadAsync(_caste.Id, _cancellationToken)).ReturnsAsync(_caste);

    _activity.Contextualize(_world);
  }

  [Fact(DisplayName = "It should return the found caste.")]
  public async Task It_should_return_the_found_caste()
  {
    ResolveCasteQuery query = new(_activity, _caste.EntityId);

    Caste caste = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_caste, caste);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(
      _activity,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Caste && y.Id == query.Id),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw AggregateNotFoundException when the caste could not be found.")]
  public async Task It_should_throw_AggregateNotFoundException_when_the_caste_could_not_be_found()
  {
    ResolveCasteQuery query = new(_activity, Guid.NewGuid());

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<Caste>>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(new CasteId(_world.Id, query.Id).Value, exception.Id);
    Assert.Equal("CasteId", exception.PropertyName);
  }
}
