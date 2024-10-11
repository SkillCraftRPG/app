using Logitar.Portal.Contracts.Search;
using Moq;
using SkillCraft.Application.Characters.Commands;
using SkillCraft.Application.Lineages;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain;
using SkillCraft.Domain.Lineages;

namespace SkillCraft.Application.Characters.Creation;

[Trait(Traits.Category, Categories.Unit)]
public class ResolveLineageQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILineageQuerier> _lineageQuerier = new();
  private readonly Mock<ILineageRepository> _lineageRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly ResolveLineageQueryHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Lineage _lineage;
  private readonly Lineage _species;
  private readonly Lineage _nation;
  private readonly CreateCharacterCommand _activity = new(new CreateCharacterPayload());

  public ResolveLineageQueryHandlerTests()
  {
    _handler = new(_lineageQuerier.Object, _lineageRepository.Object, _permissionService.Object);

    _lineage = new(_world.Id, parent: null, new Name("Dhampir"), _world.OwnerId);
    _lineageRepository.Setup(x => x.LoadAsync(_lineage.Id, _cancellationToken)).ReturnsAsync(_lineage);
    _species = new(_world.Id, parent: null, new Name("Humain"), _world.OwnerId);
    _lineageRepository.Setup(x => x.LoadAsync(_species.Id, _cancellationToken)).ReturnsAsync(_species);
    _nation = new(_world.Id, _species, new Name("Orrin"), _world.OwnerId);
    _lineageRepository.Setup(x => x.LoadAsync(_nation.Id, _cancellationToken)).ReturnsAsync(_nation);

    _activity.Contextualize(_world);
  }

  [Fact(DisplayName = "It should return the found nation.")]
  public async Task It_should_return_the_found_nation()
  {
    ResolveLineageQuery query = new(_activity, _nation.EntityId);

    Lineage lineage = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_nation, lineage);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(_activity, EntityType.Lineage, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return the found species without nations.")]
  public async Task It_should_return_the_found_species_without_nations()
  {
    ResolveLineageQuery query = new(_activity, _lineage.EntityId);

    SearchResults<LineageModel> results = new();
    _lineageQuerier.Setup(x => x.SearchAsync(_world.Id, It.Is<SearchLineagesPayload>(y => y.ParentId == query.Id), _cancellationToken)).ReturnsAsync(results);

    Lineage lineage = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_lineage, lineage);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(_activity, EntityType.Lineage, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw AggregateNotFoundException when the lineage could not be found.")]
  public async Task It_should_throw_AggregateNotFoundException_when_the_lineage_could_not_be_found()
  {
    ResolveLineageQuery query = new(_activity, Guid.NewGuid());

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<Lineage>>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(new LineageId(_world.Id, query.Id).Value, exception.Id);
    Assert.Equal("LineageId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw InvalidCharacterLineageException when the lineage (species) has children (nations).")]
  public async Task It_should_throw_InvalidCharacterLineageException_when_the_lineage_species_has_children_nations()
  {
    ResolveLineageQuery query = new(_activity, _species.EntityId);

    SearchResults<LineageModel> results = new([new LineageModel { Id = _nation.EntityId }]);
    _lineageQuerier.Setup(x => x.SearchAsync(_world.Id, It.Is<SearchLineagesPayload>(y => y.ParentId == query.Id), _cancellationToken)).ReturnsAsync(results);

    var exception = await Assert.ThrowsAsync<InvalidCharacterLineageException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(query.Id, exception.Id);
    Assert.Equal("LineageId", exception.PropertyName);
  }
}
