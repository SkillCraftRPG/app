using Logitar;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Application.Customizations.Commands;
using SkillCraft.Application.Customizations.Queries;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain;
using SkillCraft.Domain.Customizations;

namespace SkillCraft.Application.Customizations;

[Trait(Traits.Category, Categories.Integration)]
public class CustomizationTests : IntegrationTests
{
  private readonly ICustomizationRepository _customizationRepository;

  private readonly Customization _abruti;
  private readonly Customization _affiniteAnimale;
  private readonly Customization _aigrefin;
  private readonly Customization _chanceExtraordinaire;
  private readonly Customization _durACuire;

  public CustomizationTests() : base()
  {
    _customizationRepository = ServiceProvider.GetRequiredService<ICustomizationRepository>();

    _abruti = new(World.Id, CustomizationType.Disability, new Name("abruti"), UserId);
    _affiniteAnimale = new(World.Id, CustomizationType.Gift, new Name("Affinité animale"), UserId);
    _aigrefin = new(World.Id, CustomizationType.Gift, new Name("Aigrefin"), UserId);
    _chanceExtraordinaire = new(World.Id, CustomizationType.Gift, new Name("Chance extraordinaire"), UserId);
    _durACuire = new(World.Id, CustomizationType.Gift, new Name("Dur à cuire"), UserId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _customizationRepository.SaveAsync([_abruti, _affiniteAnimale, _aigrefin, _chanceExtraordinaire, _durACuire]);
  }

  [Fact(DisplayName = "It should create a new customization.")]
  public async Task It_should_create_a_new_customization()
  {
    CreateOrReplaceCustomizationPayload payload = new(" Effacé ")
    {
      Type = CustomizationType.Gift,
      Description = "  Le personnage est considéré légèrement obscurci lorsqu’il est renversé. Lorsqu’il bénéficie du demi-abri contre une attaque à distance d’une créature, alors il traite l’abri comme un abri aux trois-quarts.  "
    };

    CreateOrReplaceCustomizationCommand command = new(Guid.NewGuid(), payload, Version: null);
    CreateOrReplaceCustomizationResult result = await Pipeline.ExecuteAsync(command);
    Assert.True(result.Created);

    CustomizationModel? customization = result.Customization;
    Assert.NotNull(customization);
    Assert.Equal(command.Id, customization.Id);
    Assert.Equal(2, customization.Version);
    Assert.Equal(DateTime.UtcNow, customization.CreatedOn, TimeSpan.FromSeconds(1));
    Assert.True(customization.CreatedOn < customization.UpdatedOn);
    Assert.Equal(Actor, customization.CreatedBy);
    Assert.Equal(customization.CreatedBy, customization.UpdatedBy);

    Assert.Equal(World.Id.ToGuid(), customization.World.Id);

    Assert.Equal(payload.Type, customization.Type);
    Assert.Equal(payload.Name.Trim(), customization.Name);
    Assert.Equal(payload.Description?.CleanTrim(), customization.Description);

    Assert.NotNull(await SkillCraftContext.Customizations.AsNoTracking().SingleOrDefaultAsync(x => x.Id == customization.Id));
  }

  [Fact(DisplayName = "It should replace an existing customization.")]
  public async Task It_should_replace_an_existing_customization()
  {
    long version = _abruti.Version;

    Description description = new("Simple d’esprit et bon vivant ou complètement taré, le personnage est dénué de sens commun et n’est pas doté d’une bonne mémoire. Ses tests de _Connaissance_ ainsi que ses tests d’_Investigation_ afin de poser une conclusion sont effectués avec désavantage.");
    _abruti.Description = description;
    _abruti.Update(UserId);
    await _customizationRepository.SaveAsync(_abruti);

    CreateOrReplaceCustomizationPayload payload = new(" Abruti ")
    {
      Type = CustomizationType.Gift,
      Description = "    "
    };

    CreateOrReplaceCustomizationCommand command = new(_abruti.EntityId, payload, version);
    CreateOrReplaceCustomizationResult result = await Pipeline.ExecuteAsync(command);
    Assert.False(result.Created);

    CustomizationModel? customization = result.Customization;
    Assert.NotNull(customization);
    Assert.Equal(command.Id, customization.Id);
    Assert.Equal(_abruti.Version + 1, customization.Version);
    Assert.Equal(DateTime.UtcNow, customization.UpdatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, customization.UpdatedBy);

    Assert.Equal(_abruti.Type, customization.Type);
    Assert.Equal(payload.Name.Trim(), customization.Name);
    Assert.Equal(description.Value, customization.Description);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task It_should_return_empty_search_results()
  {
    SearchCustomizationsPayload payload = new();
    payload.Search.Terms.Add(new SearchTerm("test"));

    SearchCustomizationsQuery query = new(payload);
    SearchResults<CustomizationModel> results = await Pipeline.ExecuteAsync(query);

    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    SearchCustomizationsPayload payload = new()
    {
      Type = CustomizationType.Gift,
      Skip = 1,
      Limit = 1
    };
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.Add(new SearchTerm("%f%"));
    payload.Search.Terms.Add(new SearchTerm("%u%"));
    payload.Sort.Add(new CustomizationSortOption(CustomizationSort.Name, isDescending: true));

    payload.Ids.Add(Guid.Empty);
    payload.Ids.AddRange((await _customizationRepository.LoadAsync()).Select(customization => customization.EntityId));
    payload.Ids.Remove(_aigrefin.EntityId);

    SearchCustomizationsQuery query = new(payload);
    SearchResults<CustomizationModel> results = await Pipeline.ExecuteAsync(query);

    Assert.Equal(2, results.Total);
    CustomizationModel customization = Assert.Single(results.Items);
    Assert.Equal(_affiniteAnimale.EntityId, customization.Id);
  }

  [Fact(DisplayName = "It should return the customization found by ID.")]
  public async Task It_should_return_the_customization_found_by_Id()
  {
    ReadCustomizationQuery query = new(_aigrefin.EntityId);
    CustomizationModel? customization = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(customization);
    Assert.Equal(_aigrefin.EntityId, customization.Id);
  }

  [Fact(DisplayName = "It should update an existing customization.")]
  public async Task It_should_update_an_existing_customization()
  {
    UpdateCustomizationPayload payload = new()
    {
      Description = new Change<string>("  Simple d’esprit et bon vivant ou complètement taré, le personnage est dénué de sens commun et n’est pas doté d’une bonne mémoire. Ses tests de _Connaissance_ ainsi que ses tests d’_Investigation_ afin de poser une conclusion sont effectués avec désavantage.  ")
    };

    UpdateCustomizationCommand command = new(_abruti.EntityId, payload);
    CustomizationModel? customization = await Pipeline.ExecuteAsync(command);

    Assert.NotNull(customization);
    Assert.Equal(command.Id, customization.Id);
    Assert.Equal(_abruti.Version + 1, customization.Version);
    Assert.Equal(DateTime.UtcNow, customization.UpdatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, customization.UpdatedBy);

    Assert.Equal(_abruti.Type, customization.Type);
    Assert.Equal(_abruti.Name.Value, customization.Name);
    Assert.NotNull(payload.Description.Value);
    Assert.Equal(payload.Description.Value.CleanTrim(), customization.Description);
  }
}
