using Logitar.Portal.Contracts.Search;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Contracts.Castes;
using SkillCraft.Domain;
using SkillCraft.Domain.Castes;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Castes.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchCastesQueryTests : IntegrationTests
{
  private readonly ICasteRepository _casteRepository;
  private readonly IWorldRepository _worldRepository;

  private readonly World _otherWorld;

  private readonly Caste _caste1;
  private readonly Caste _caste2;
  private readonly Caste _caste3;
  private readonly Caste _caste4;
  private readonly Caste _caste5;

  public SearchCastesQueryTests() : base()
  {
    _casteRepository = ServiceProvider.GetRequiredService<ICasteRepository>();
    _worldRepository = ServiceProvider.GetRequiredService<IWorldRepository>();

    _otherWorld = new(new Slug("tir-na-nog"), World.OwnerId);

    _caste1 = new(World.Id, new Name("Amuseur"), World.OwnerId);
    _caste2 = new(World.Id, new Name("Artisan"), World.OwnerId);
    _caste3 = new(World.Id, new Name("Bohème"), World.OwnerId);
    _caste4 = new(World.Id, new Name("Exilé"), World.OwnerId);
    _caste5 = new(_otherWorld.Id, new Name("Exilé"), _otherWorld.OwnerId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _worldRepository.SaveAsync(_otherWorld);
    await _casteRepository.SaveAsync([_caste1, _caste2, _caste3, _caste4, _caste5]);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task It_should_return_empty_search_results()
  {
    SearchCastesPayload payload = new()
    {
      Search = new TextSearch([new SearchTerm("%test%")])
    };

    SearchCastesQuery query = new(payload);

    SearchResults<CasteModel> castes = await Pipeline.ExecuteAsync(query);
    Assert.Equal(0, castes.Total);
    Assert.Empty(castes.Items);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    SearchCastesPayload payload = new()
    {
      Search = new TextSearch([new SearchTerm("a%"), new SearchTerm("%x%")], SearchOperator.Or),
      Sort = [new CasteSortOption(CasteSort.Name, isDescending: true)],
      Skip = 1,
      Limit = 1,
      Ids = (await _casteRepository.LoadAsync()).Select(x => x.Id.ToGuid()).ToList()
    };
    payload.Ids.Remove(_caste1.Id.ToGuid());
    payload.Ids.Add(Guid.Empty);

    SearchCastesQuery query = new(payload);

    SearchResults<CasteModel> castes = await Pipeline.ExecuteAsync(query);
    Assert.Equal(2, castes.Total);

    CasteModel caste = Assert.Single(castes.Items);
    Assert.Equal(_caste2.Id.ToGuid(), caste.Id);
  }
}
