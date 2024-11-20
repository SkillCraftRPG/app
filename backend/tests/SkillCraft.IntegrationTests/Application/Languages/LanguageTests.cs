using Logitar;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Application.Languages.Commands;
using SkillCraft.Application.Languages.Queries;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Languages;
using SkillCraft.Domain;
using SkillCraft.Domain.Languages;

namespace SkillCraft.Application.Languages;

[Trait(Traits.Category, Categories.Integration)]
public class LanguageTests : IntegrationTests
{
  private readonly ILanguageRepository _languageRepository;

  private readonly Script _script = new("Cunéiforme");
  private readonly Language _ashtavrin;
  private readonly Language _cassite;
  private readonly Language _isilien;
  private readonly Language _kroumien;
  private readonly Language _squerzele;

  public LanguageTests() : base()
  {
    _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();

    _ashtavrin = new(World.Id, new Name("Ashtavrin"), UserId)
    {
      Script = _script,
      TypicalSpeakers = new TypicalSpeakers("Ashtavrins")
    };
    _ashtavrin.Update(UserId);
    _cassite = new(World.Id, new Name("Cassite"), UserId)
    {
      Script = _script,
      TypicalSpeakers = new TypicalSpeakers("Chalites, Nains bruns, Savákti")
    };
    _cassite.Update(UserId);
    _isilien = new(World.Id, new Name("Isilien"), UserId)
    {
      Script = new Script("Elfique"),
      TypicalSpeakers = new TypicalSpeakers("Elfes lunaires")
    };
    _isilien.Update(UserId);
    _kroumien = new(World.Id, new Name("Kroumien"), UserId)
    {
      Script = _script,
      TypicalSpeakers = new TypicalSpeakers("Nains bruns")
    };
    _kroumien.Update(UserId);
    _squerzele = new(World.Id, new Name("Squerzèle"), UserId)
    {
      Script = _script,
      TypicalSpeakers = new TypicalSpeakers("Nains gris")
    };
    _squerzele.Update(UserId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _languageRepository.SaveAsync([_ashtavrin, _cassite, _isilien, _kroumien, _squerzele]);
  }

  [Fact(DisplayName = "It should create a new language.")]
  public async Task It_should_create_a_new_language()
  {
    CreateOrReplaceLanguagePayload payload = new(" Orrinique ")
    {
      Description = "  Cette langue est issue des Orrins. Elle est très utilisée en Ouespéro en raison de l’histoire des Orrins. En effet, il s’agit d’une des premières civilisations du continent, leur culture et leur influence sont étendues aux confins de l’Ouespéro.  ",
      Script = " Orrinique ",
      TypicalSpeakers = "  Chalites, Minotaures, Orrins, Satyres, Sophitéons  "
    };

    CreateOrReplaceLanguageCommand command = new(Guid.NewGuid(), payload, Version: null);
    CreateOrReplaceLanguageResult result = await Pipeline.ExecuteAsync(command);
    Assert.True(result.Created);

    LanguageModel? language = result.Language;
    Assert.NotNull(language);
    Assert.Equal(command.Id, language.Id);
    Assert.Equal(2, language.Version);
    Assert.Equal(DateTime.UtcNow, language.CreatedOn, TimeSpan.FromSeconds(1));
    Assert.True(language.CreatedOn < language.UpdatedOn);
    Assert.Equal(Actor, language.CreatedBy);
    Assert.Equal(language.CreatedBy, language.UpdatedBy);

    Assert.Equal(World.Id.ToGuid(), language.World.Id);

    Assert.Equal(payload.Name.Trim(), language.Name);
    Assert.Equal(payload.Description?.CleanTrim(), language.Description);

    Assert.Equal(payload.Script.Trim(), language.Script);
    Assert.Equal(payload.TypicalSpeakers.Trim(), language.TypicalSpeakers);

    Assert.NotNull(await SkillCraftContext.Languages.AsNoTracking().SingleOrDefaultAsync(x => x.Id == language.Id));
  }

  [Fact(DisplayName = "It should list the scripts.")]
  public async Task It_should_list_the_scripts()
  {
    SearchScriptsQuery query = new();
    SearchResults<string> results = await Pipeline.ExecuteAsync(query);

    Assert.Equal(2, results.Total);
    Assert.Equal([_script.Value, "Elfique"], results.Items);
  }

  [Fact(DisplayName = "It should replace an existing language.")]
  public async Task It_should_replace_an_existing_language()
  {
    long version = _isilien.Version;

    Description description = new("  Cette langue est parlée par les Elfes lunaires peuplant les taïgas, marais et montagnes du Nord du continent. Elle est peu utilisée à l’extérieur de cette région, d’autant plus que les Elfes lunaires ne la partagent pas aux étrangers.  ");
    _isilien.Description = description;
    _isilien.Update(UserId);
    await _languageRepository.SaveAsync(_isilien);

    CreateOrReplaceLanguagePayload payload = new(" Isilien ")
    {
      Description = "    ",
      Script = " Elfique ",
      TypicalSpeakers = "    "
    };

    CreateOrReplaceLanguageCommand command = new(_isilien.EntityId, payload, version);
    CreateOrReplaceLanguageResult result = await Pipeline.ExecuteAsync(command);
    Assert.False(result.Created);

    LanguageModel? language = result.Language;
    Assert.NotNull(language);
    Assert.Equal(command.Id, language.Id);
    Assert.Equal(_isilien.Version + 1, language.Version);
    Assert.Equal(DateTime.UtcNow, language.UpdatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, language.UpdatedBy);

    Assert.Equal(payload.Name.Trim(), language.Name);
    Assert.Equal(description.Value, language.Description);

    Assert.Equal(payload.Script.Trim(), language.Script);
    Assert.Null(language.TypicalSpeakers);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task It_should_return_empty_search_results()
  {
    SearchLanguagesPayload payload = new();
    payload.Search.Terms.Add(new SearchTerm("test"));

    SearchLanguagesQuery query = new(payload);
    SearchResults<LanguageModel> results = await Pipeline.ExecuteAsync(query);

    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    SearchLanguagesPayload payload = new()
    {
      Script = _script.Value,
      Skip = 1,
      Limit = 1
    };
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.Add(new SearchTerm("%elfe%"));
    payload.Search.Terms.Add(new SearchTerm("%nain%"));
    payload.Sort.Add(new LanguageSortOption(LanguageSort.Name, isDescending: true));

    payload.Ids.Add(Guid.Empty);
    payload.Ids.AddRange((await _languageRepository.LoadAsync()).Select(language => language.EntityId));
    payload.Ids.Remove(_cassite.EntityId);

    SearchLanguagesQuery query = new(payload);
    SearchResults<LanguageModel> results = await Pipeline.ExecuteAsync(query);

    Assert.Equal(2, results.Total);
    LanguageModel language = Assert.Single(results.Items);
    Assert.Equal(_kroumien.EntityId, language.Id);
  }

  [Fact(DisplayName = "It should return the language found by ID.")]
  public async Task It_should_return_the_language_found_by_Id()
  {
    ReadLanguageQuery query = new(_isilien.EntityId);
    LanguageModel? language = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(language);
    Assert.Equal(_isilien.EntityId, language.Id);
  }

  [Fact(DisplayName = "It should update an existing language.")]
  public async Task It_should_update_an_existing_language()
  {
    UpdateLanguagePayload payload = new()
    {
      Description = new Change<string>("  Le Squerzèle est parlé par les nains gris répandus dans la grande plaine d’Ouespéro. En raison des bonnes relations des Nains gris avec les autres peuples ainsi que du fait qu’ils s’établissent un peu partout, cette langue est très répandue et utilisée auprès des artisans et des marchands désirant commercer avec les Nains gris.  ")
    };

    UpdateLanguageCommand command = new(_squerzele.EntityId, payload);
    LanguageModel? language = await Pipeline.ExecuteAsync(command);

    Assert.NotNull(language);
    Assert.Equal(command.Id, language.Id);
    Assert.Equal(_squerzele.Version + 1, language.Version);
    Assert.Equal(DateTime.UtcNow, language.UpdatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, language.UpdatedBy);

    Assert.Equal(_squerzele.Name.Value, language.Name);
    Assert.NotNull(payload.Description.Value);
    Assert.Equal(payload.Description.Value.CleanTrim(), language.Description);

    Assert.Equal(_squerzele.Script?.Value, language.Script);
    Assert.Equal(_squerzele.TypicalSpeakers?.Value, language.TypicalSpeakers);
  }
}
