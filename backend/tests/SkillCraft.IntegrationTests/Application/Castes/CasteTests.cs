using Logitar;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Application.Castes.Commands;
using SkillCraft.Application.Castes.Queries;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Castes;
using SkillCraft.Domain;
using SkillCraft.Domain.Castes;

namespace SkillCraft.Application.Castes;

[Trait(Traits.Category, Categories.Unit)]
public class CasteTests : IntegrationTests
{
  private readonly ICasteRepository _casteRepository;

  private readonly Caste _amuseur;
  private readonly Caste _boheme;
  private readonly Caste _exile;
  private readonly Caste _guerisseur;

  public CasteTests() : base()
  {
    _casteRepository = ServiceProvider.GetRequiredService<ICasteRepository>();

    _amuseur = new(World.Id, new Name("Amuseur"), UserId);
    _boheme = new(World.Id, new Name("Bohème"), UserId);
    _exile = new(World.Id, new Name("Exilé"), UserId)
    {
      Skill = Skill.Survival,
      WealthRoll = new Roll("6d6")
    };
    _exile.AddTrait(new Trait(new Name("Rancunier"), new Description("Peu importe la raison, le personnage a renoncé à sa fidélité envers son seigneur. Il connaît les rouages du système lorsqu’il est question d’éviter de payer des taxes et de jouer avec les lois en place. Lorsqu’il se trouve dans son pays d’origine, il peut réduire ses dépenses essentielles de 10 %.")));
    _exile.AddTrait(new Trait(new Name("Vagabond"), new Description("Aucun village ni aucune ville n’est un meilleur domicile pour le personnage que la route. Qu’il soit nomade par choix ou par obligation, ses tests de Survie afin de trouver de l’eau, de la nourriture ou un abri se voient conférer l’avantage lorsqu’il se trouve à proximité d’une route maintenue.")));
    _exile.Update(UserId);
    _guerisseur = new(World.Id, new Name("Guérisseur"), UserId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _casteRepository.SaveAsync([_amuseur, _boheme, _exile, _guerisseur]);
  }

  [Fact(DisplayName = "It should create a new caste.")]
  public async Task It_should_create_a_new_caste()
  {
    SaveCastePayload payload = new(" Artisan ")
    {
      Description = "  L’artisan est un expert d’un procédé de transformation des matières brutes. Il peut être un boulanger, un forgeron, un orfèvre, un tisserand ou pratiquer tout genre de profession œuvrant dans la transformation des matières brutes.  ",
      Skill = Skill.Craft,
      WealthRoll = "8d6"
    };
    payload.Traits.Add(new TraitPayload("Professionnel")
    {
      Description = "Les apprentissages et réalisations du personnage lui ont permis de devenir membre d’une organisation de professionnels comme lui, telle une guilde d’artisans ou de marchands. S’il ne peut payer pour un toit ou de la nourriture, il peut facilement trouver du travail afin de couvrir ces dépenses essentielles."
    });
    payload.Traits.Add(new TraitPayload("Sujet")
    {
      Id = Guid.NewGuid(),
      Description = "Sujet d’un seigneur quelconque, le personnage n’est victime d’aucune taxe imposée aux voyageurs étrangers. Il peut réduire ses dépenses essentielles de 10 % sur sa terre natale."
    });

    SaveCasteCommand command = new(Guid.NewGuid(), payload, Version: null);
    SaveCasteResult result = await Pipeline.ExecuteAsync(command);
    Assert.True(result.Created);

    CasteModel? caste = result.Caste;
    Assert.NotNull(caste);
    Assert.Equal(command.Id, caste.Id);
    Assert.Equal(2, caste.Version);
    Assert.Equal(DateTime.UtcNow, caste.CreatedOn, TimeSpan.FromSeconds(1));
    Assert.True(caste.CreatedOn < caste.UpdatedOn);
    Assert.Equal(Actor, caste.CreatedBy);
    Assert.Equal(caste.CreatedBy, caste.UpdatedBy);

    Assert.Equal(World.Id.ToGuid(), caste.World.Id);

    Assert.Equal(payload.Name.Trim(), caste.Name);
    Assert.Equal(payload.Description?.CleanTrim(), caste.Description);

    Assert.Equal(payload.Skill, caste.Skill);
    Assert.Equal(payload.WealthRoll, caste.WealthRoll);

    Assert.Equal(payload.Traits.Count, caste.Traits.Count);
    foreach (TraitPayload trait in payload.Traits)
    {
      Assert.Contains(caste.Traits, t => (trait.Id == null || trait.Id == t.Id) && t.Name == trait.Name && t.Description == trait.Description);
    }

    Assert.NotNull(await SkillCraftContext.Castes.AsNoTracking().SingleOrDefaultAsync(x => x.Id == caste.Id));
  }

  [Fact(DisplayName = "It should replace an existing caste.")]
  public async Task It_should_replace_an_existing_caste()
  {
    long version = _amuseur.Version;

    Description description = new("  Adepte d’art, de poésie, de cirque ou de théâtre, l’amuseur ère de villages en cités afin d’offrir ses performances. Bien plus qu’un simple gagne-pain, cela lui permet de former un réseau de contacts et de recueillir toutes sortes d’informations.  ");
    _amuseur.Description = description;
    _amuseur.Update(UserId);
    await _casteRepository.SaveAsync(_amuseur);

    SaveCastePayload payload = new(" Amuseur ")
    {
      Description = "    ",
      Skill = Skill.Performance,
      WealthRoll = "8d6"
    };
    payload.Traits.Add(new TraitPayload("Protégé")
    {
      Description = "Par le prestige de sa famille et sa participation à des événements de l’aristocratie, le personnage est connu de la noblesse locale. Il peut demander gîte et couvert membres de la noblesse. En échange, ceux-ci peuvent lui demander une ou plusieurs faveurs immédiates, futures ou encore invoquer le droit de surprise, c’est-à-dire une faveur d’un quelconque ordre de grandeur lorsque l’occasion de présenter."
    });
    payload.Traits.Add(new TraitPayload("Vagabond")
    {
      Id = Guid.NewGuid(),
      Description = "Aucun village ni aucune ville n’est un meilleur domicile pour le personnage que la route. Qu’il soit nomade par choix ou par obligation, ses tests de Survie afin de trouver de l’eau, de la nourriture ou un abri se voient conférer l’avantage lorsqu’il se trouve à proximité d’une route maintenue."
    });

    SaveCasteCommand command = new(_amuseur.EntityId, payload, version);
    SaveCasteResult result = await Pipeline.ExecuteAsync(command);
    Assert.False(result.Created);

    CasteModel? caste = result.Caste;
    Assert.NotNull(caste);
    Assert.Equal(command.Id, caste.Id);
    Assert.Equal(_amuseur.Version + 1, caste.Version);
    Assert.Equal(DateTime.UtcNow, caste.UpdatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, caste.UpdatedBy);

    Assert.Equal(payload.Name.Trim(), caste.Name);
    Assert.Equal(description.Value, caste.Description);

    Assert.Equal(payload.Skill, caste.Skill);
    Assert.Equal(payload.WealthRoll, caste.WealthRoll);

    Assert.Equal(payload.Traits.Count, caste.Traits.Count);
    foreach (TraitPayload trait in payload.Traits)
    {
      Assert.Contains(caste.Traits, t => (trait.Id == null || trait.Id == t.Id) && t.Name == trait.Name && t.Description == trait.Description);
    }
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task It_should_return_empty_search_results()
  {
    SearchCastesPayload payload = new();
    payload.Search.Terms.Add(new SearchTerm("test"));

    SearchCastesQuery query = new(payload);
    SearchResults<CasteModel> results = await Pipeline.ExecuteAsync(query);

    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    SearchCastesPayload payload = new()
    {
      Skip = 1,
      Limit = 1
    };
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.Add(new SearchTerm("%o%"));
    payload.Search.Terms.Add(new SearchTerm("%u%"));
    payload.Sort.Add(new CasteSortOption(CasteSort.Name, isDescending: false));

    payload.Ids.Add(Guid.Empty);
    payload.Ids.AddRange((await _casteRepository.LoadAsync()).Select(caste => caste.EntityId));
    payload.Ids.Remove(_boheme.EntityId);

    SearchCastesQuery query = new(payload);
    SearchResults<CasteModel> results = await Pipeline.ExecuteAsync(query);

    Assert.Equal(2, results.Total);
    CasteModel caste = Assert.Single(results.Items);
    Assert.Equal(_guerisseur.EntityId, caste.Id);
  }

  [Fact(DisplayName = "It should return the caste found by ID.")]
  public async Task It_should_return_the_caste_found_by_Id()
  {
    ReadCasteQuery query = new(_boheme.EntityId);
    CasteModel? caste = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(caste);
    Assert.Equal(_boheme.EntityId, caste.Id);
  }

  [Fact(DisplayName = "It should update an existing caste.")]
  public async Task It_should_update_an_existing_caste()
  {
    UpdateCastePayload payload = new()
    {
      Description = new Change<string>("  Banni par son peuple ou par choix personnel, l’exilé a recours à son instinct de survie et à sa connaissance des milieux sauvages afin de survivre à son environnement. Il garde de mauvais souvenirs de son ancienne vie et prépare aigrement sa vengeance.  ")
    };

    UpdateCasteCommand command = new(_exile.EntityId, payload);
    CasteModel? caste = await Pipeline.ExecuteAsync(command);

    Assert.NotNull(caste);
    Assert.Equal(command.Id, caste.Id);
    Assert.Equal(_exile.Version + 1, caste.Version);
    Assert.Equal(DateTime.UtcNow, caste.UpdatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, caste.UpdatedBy);

    Assert.Equal(_exile.Name.Value, caste.Name);
    Assert.NotNull(payload.Description.Value);
    Assert.Equal(payload.Description.Value.CleanTrim(), caste.Description);

    Assert.Equal(_exile.Skill, caste.Skill);
    Assert.Equal(_exile.WealthRoll?.Value, caste.WealthRoll);

    Assert.Equal(_exile.Traits.Count, caste.Traits.Count);
    foreach (KeyValuePair<Guid, Trait> trait in _exile.Traits)
    {
      Assert.Contains(caste.Traits, t => t.Id == trait.Key && t.Name == trait.Value.Name.Value && t.Description == trait.Value.Description?.Value);
    }
  }
}
