using Logitar.Portal.Contracts.Search;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchWorldsQueryTests : IntegrationTests
{
  private readonly IWorldRepository _worldRepository;

  private readonly World _world1;
  private readonly World _world2;
  private readonly World _world3;

  public SearchWorldsQueryTests() : base()
  {
    _worldRepository = ServiceProvider.GetRequiredService<IWorldRepository>();

    UserId userId = new(Actor.Id);
    _world1 = new(new Slug("Mwnga"), userId);
    _world2 = new(new Slug("Hyrule"), userId);
    _world3 = new(new Slug("JOURUNGAR"), userId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _worldRepository.SaveAsync([_world1, _world2, _world3]);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task It_should_return_empty_search_results()
  {
    SearchWorldsPayload payload = new()
    {
      Search = new TextSearch([new SearchTerm("%test%")])
    };

    SearchWorldsQuery query = new(payload);

    SearchResults<WorldModel> worlds = await Pipeline.ExecuteAsync(query);
    Assert.Equal(0, worlds.Total);
    Assert.Empty(worlds.Items);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    SearchWorldsPayload payload = new()
    {
      Search = new TextSearch(
      [
        new SearchTerm("%nga%")
      ], SearchOperator.Or),
      Sort = [new WorldSortOption(WorldSort.Name, isDescending: true)],
      Skip = 1,
      Limit = 1,
      Ids = (await _worldRepository.LoadAsync()).Select(x => x.Id.ToGuid()).ToList()
    };
    payload.Ids.Remove(_world1.Id.ToGuid());
    payload.Ids.Add(Guid.Empty);

    SearchWorldsQuery query = new(payload);

    SearchResults<WorldModel> worlds = await Pipeline.ExecuteAsync(query);
    Assert.Equal(2, worlds.Total);

    WorldModel world = Assert.Single(worlds.Items);
    Assert.Equal(_world3.Id.ToGuid(), world.Id);
  }
}
