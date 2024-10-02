using Logitar;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Application.Educations.Commands;
using SkillCraft.Application.Educations.Queries;
using SkillCraft.Contracts;
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
    _champsBataille = new(World.Id, new Name("Champs de bataille"), UserId)
    {
      Skill = Skill.Resistance,
      WealthMultiplier = 4.0
    };
    _champsBataille.Update(UserId);
    _dansLaRue = new(World.Id, new Name("Dans la rue"), UserId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _educationRepository.SaveAsync([_apprentiEtranger, _apprentiMaitre, _champsBataille, _dansLaRue]);
  }

  [Fact(DisplayName = "It should create a new education.")]
  public async Task It_should_create_a_new_education()
  {
    SaveEducationPayload payload = new(" Classique ")
    {
      Description = "  Peu peuvent se vanter d’avoir reçu une éducation traditionnelle comme celle du personnage. Il a suivi un parcours scolaire conforme et sans dérogation ayant mené à une instruction de haute qualité. Malgré son manque d’expériences personnelles, son grand savoir lui permet de se débrouiller même dans les situations les plus difficiles.  ",
      Skill = Skill.Knowledge,
      WealthMultiplier = 12.0
    };

    SaveEducationCommand command = new(Guid.NewGuid(), payload, Version: null);
    SaveEducationResult result = await Pipeline.ExecuteAsync(command);
    Assert.True(result.Created);

    EducationModel? education = result.Education;
    Assert.NotNull(education);
    Assert.Equal(command.Id, education.Id);
    Assert.Equal(2, education.Version);
    Assert.Equal(DateTime.UtcNow, education.CreatedOn, TimeSpan.FromSeconds(1));
    Assert.True(education.CreatedOn < education.UpdatedOn);
    Assert.Equal(Actor, education.CreatedBy);
    Assert.Equal(education.CreatedBy, education.UpdatedBy);

    Assert.Equal(World.Id.ToGuid(), education.World.Id);

    Assert.Equal(payload.Name.Trim(), education.Name);
    Assert.Equal(payload.Description?.CleanTrim(), education.Description);

    Assert.Equal(payload.Skill, education.Skill);
    Assert.Equal(payload.WealthMultiplier, education.WealthMultiplier);

    Assert.NotNull(await SkillCraftContext.Educations.AsNoTracking().SingleOrDefaultAsync(x => x.Id == education.Id));
  }

  [Fact(DisplayName = "It should replace an existing education.")]
  public async Task It_should_replace_an_existing_education()
  {
    long version = _apprentiMaitre.Version;

    Description description = new("  Le personnage a tout appris d’un maître partageant un art ou une profession avec lui. Ce maître lui a enseigné les subtilités qui permettront au personnage d’être éventuellement reconnu comme un maître lui aussi. Cet apprentissage a été effectué dans l’ordre et la discipline, sous la direction du maître.  ");
    _apprentiMaitre.Description = description;
    _apprentiMaitre.Update(UserId);
    await _educationRepository.SaveAsync(_apprentiMaitre);

    SaveEducationPayload payload = new(" Apprenti d’un maître ")
    {
      Description = "    ",
      Skill = Skill.Discipline,
      WealthMultiplier = 12.0
    };

    SaveEducationCommand command = new(_apprentiMaitre.EntityId, payload, version);
    SaveEducationResult result = await Pipeline.ExecuteAsync(command);
    Assert.False(result.Created);

    EducationModel? education = result.Education;
    Assert.NotNull(education);
    Assert.Equal(command.Id, education.Id);
    Assert.Equal(_apprentiMaitre.Version + 1, education.Version);
    Assert.Equal(DateTime.UtcNow, education.UpdatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, education.UpdatedBy);

    Assert.Equal(payload.Name.Trim(), education.Name);
    Assert.Equal(description.Value, education.Description);

    Assert.Equal(payload.Skill, education.Skill);
    Assert.Equal(payload.WealthMultiplier, education.WealthMultiplier);
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

  [Fact(DisplayName = "It should update an existing education.")]
  public async Task It_should_update_an_existing_education()
  {
    UpdateEducationPayload payload = new()
    {
      Description = new Change<string>("  Le personnage est un enfant de la guerre, un fugitif d’un conflit ayant affecté les civils. Son enfance est maigre de moments heureux et aisés, ce a fait de lui une personne endurcie dotée d’une personnalité pragmatique.  ")
    };

    UpdateEducationCommand command = new(_champsBataille.EntityId, payload);
    EducationModel? education = await Pipeline.ExecuteAsync(command);

    Assert.NotNull(education);
    Assert.Equal(command.Id, education.Id);
    Assert.Equal(_champsBataille.Version + 1, education.Version);
    Assert.Equal(DateTime.UtcNow, education.UpdatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, education.UpdatedBy);

    Assert.Equal(_champsBataille.Name.Value, education.Name);
    Assert.NotNull(payload.Description.Value);
    Assert.Equal(payload.Description.Value.CleanTrim(), education.Description);

    Assert.Equal(_champsBataille.Skill, education.Skill);
    Assert.Equal(_champsBataille.WealthMultiplier, education.WealthMultiplier);
  }
}
