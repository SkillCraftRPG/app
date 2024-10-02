using Logitar.Portal.Contracts.Search;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Application.Educations.Queries;
using SkillCraft.Contracts.Educations;
using SkillCraft.Domain;
using SkillCraft.Domain.Educations;

namespace SkillCraft.Application.Educations;

[Trait(Traits.Category, Categories.Unit)]
public class EducationTests : IntegrationTests
{
  private readonly IEducationRepository _educationRepository;

  private readonly Education _apprentiEtranger;
  private readonly Education _apprentiMaitre;
  private readonly Education _champsBataille;
  private readonly Education _dansLaRue;

  public EducationTests() : base()
  {
    _educationRepository = ServiceProvider.GetRequiredService<IEducationRepository>();

    _apprentiEtranger = new(World.Id, new Name("Apprenti à l’étranger"), UserId);
    _apprentiMaitre = new(World.Id, new Name("Apprenti d’un maître"), UserId);
    _champsBataille = new(World.Id, new Name("Champs de bataille"), UserId);
    _dansLaRue = new(World.Id, new Name("Dans la rue"), UserId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _educationRepository.SaveAsync([_apprentiEtranger, _apprentiMaitre, _champsBataille, _dansLaRue]);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task It_should_return_empty_search_results()
  {
    SearchEducationsPayload payload = new();
    payload.Search.Terms.Add(new SearchTerm("test"));

    SearchEducationsQuery query = new(payload);
    SearchResults<EducationModel> results = await Pipeline.ExecuteAsync(query);

    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    SearchEducationsPayload payload = new()
    {
      Skip = 1,
      Limit = 1
    };
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.Add(new SearchTerm("%apprenti%"));
    payload.Search.Terms.Add(new SearchTerm("%bataille%"));
    payload.Sort.Add(new EducationSortOption(EducationSort.Name, isDescending: true));

    payload.Ids.Add(Guid.Empty);
    payload.Ids.AddRange((await _educationRepository.LoadAsync()).Select(education => education.EntityId));
    payload.Ids.Remove(_apprentiMaitre.EntityId);

    SearchEducationsQuery query = new(payload);
    SearchResults<EducationModel> results = await Pipeline.ExecuteAsync(query);

    Assert.Equal(2, results.Total);
    EducationModel education = Assert.Single(results.Items);
    Assert.Equal(_apprentiEtranger.EntityId, education.Id);
  }

  [Fact(DisplayName = "It should return the education found by ID.")]
  public async Task It_should_return_the_education_found_by_Id()
  {
    ReadEducationQuery query = new(_dansLaRue.EntityId);
    EducationModel? education = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(education);
    Assert.Equal(_dansLaRue.EntityId, education.Id);
  }
}
