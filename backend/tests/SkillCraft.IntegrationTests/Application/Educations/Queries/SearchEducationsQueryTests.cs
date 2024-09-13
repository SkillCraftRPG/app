using Logitar.Portal.Contracts.Search;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Contracts.Educations;
using SkillCraft.Domain;
using SkillCraft.Domain.Educations;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Educations.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchEducationsQueryTests : IntegrationTests
{
  private readonly IEducationRepository _educationRepository;
  private readonly IWorldRepository _worldRepository;

  private readonly World _otherWorld;

  private readonly Education _education1;
  private readonly Education _education2;
  private readonly Education _education3;
  private readonly Education _education4;
  private readonly Education _education5;

  public SearchEducationsQueryTests() : base()
  {
    _educationRepository = ServiceProvider.GetRequiredService<IEducationRepository>();
    _worldRepository = ServiceProvider.GetRequiredService<IWorldRepository>();

    _otherWorld = new(new Slug("tir-na-nog"), World.OwnerId);

    _education1 = new(World.Id, new Name("Apprenti à l’étranger"), World.OwnerId);
    _education2 = new(World.Id, new Name("Apprenti d’un maître"), World.OwnerId);
    _education3 = new(World.Id, new Name("Champs de bataille"), World.OwnerId);
    _education4 = new(World.Id, new Name("Classique"), World.OwnerId);
    _education5 = new(_otherWorld.Id, new Name("Classique"), _otherWorld.OwnerId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _worldRepository.SaveAsync(_otherWorld);
    await _educationRepository.SaveAsync([_education1, _education2, _education3, _education4, _education5]);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task It_should_return_empty_search_results()
  {
    SearchEducationsPayload payload = new()
    {
      Search = new TextSearch([new SearchTerm("%test%")])
    };

    SearchEducationsQuery query = new(payload);

    SearchResults<EducationModel> educations = await Pipeline.ExecuteAsync(query);
    Assert.Equal(0, educations.Total);
    Assert.Empty(educations.Items);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    SearchEducationsPayload payload = new()
    {
      Search = new TextSearch([new SearchTerm("apprenti%"), new SearchTerm("classique")], SearchOperator.Or),
      Sort = [new EducationSortOption(EducationSort.Name, isDescending: true)],
      Skip = 1,
      Limit = 1,
      Ids = (await _educationRepository.LoadAsync()).Select(x => x.Id.ToGuid()).ToList()
    };
    payload.Ids.Remove(_education4.Id.ToGuid());
    payload.Ids.Add(Guid.Empty);

    SearchEducationsQuery query = new(payload);

    SearchResults<EducationModel> educations = await Pipeline.ExecuteAsync(query);
    Assert.Equal(2, educations.Total);

    EducationModel education = Assert.Single(educations.Items);
    Assert.Equal(_education1.Id.ToGuid(), education.Id);
  }
}
