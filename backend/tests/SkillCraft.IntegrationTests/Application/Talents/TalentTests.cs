using Logitar;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Application.Talents.Commands;
using SkillCraft.Application.Talents.Queries;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Talents;
using SkillCraft.Domain;
using SkillCraft.Domain.Talents;

namespace SkillCraft.Application.Talents;

[Trait(Traits.Category, Categories.Integration)]
public class TalentTests : IntegrationTests
{
  private readonly ITalentRepository _talentRepository;

  private readonly Talent _attaqueNeutralisante;
  private readonly Talent _charge;
  private readonly Talent _coupDeBouclier;
  private readonly Talent _cuirasse;
  private readonly Talent _formationMartiale;
  private readonly Talent _manœuvresDeCombat;
  private readonly Talent _melee;

  public TalentTests() : base()
  {
    _talentRepository = ServiceProvider.GetRequiredService<ITalentRepository>();

    _melee = new(World.Id, tier: 0, new Name("Mêlée"), UserId);

    _formationMartiale = new(World.Id, tier: 0, new Name("Formation martiale"), UserId);
    _formationMartiale.SetRequiredTalent(_melee);
    _formationMartiale.Update(UserId);

    _attaqueNeutralisante = new(World.Id, tier: 1, new Name("Attaque neutralisante"), UserId);
    _attaqueNeutralisante.SetRequiredTalent(_melee);
    _attaqueNeutralisante.Update(UserId);

    _charge = new(World.Id, tier: 1, new Name("Charge"), UserId);
    _charge.SetRequiredTalent(_melee);
    _charge.Update(UserId);

    _coupDeBouclier = new(World.Id, tier: 1, new Name("Coup de bouclier"), UserId);
    _coupDeBouclier.SetRequiredTalent(_melee);
    _coupDeBouclier.Update(UserId);

    _cuirasse = new(World.Id, tier: 1, new Name("Cuirassé"), UserId);
    _cuirasse.SetRequiredTalent(_formationMartiale);
    _cuirasse.Update(UserId);

    _manœuvresDeCombat = new(World.Id, tier: 1, new Name("Manœuvres de combat"), UserId)
    {
      AllowMultiplePurchases = true
    };
    _manœuvresDeCombat.SetRequiredTalent(_melee);
    _manœuvresDeCombat.Update(UserId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _talentRepository.SaveAsync([_melee, _formationMartiale, _attaqueNeutralisante, _charge, _coupDeBouclier, _cuirasse, _manœuvresDeCombat]);
  }

  [Fact(DisplayName = "It should create a new talent.")]
  public async Task It_should_create_a_new_talent()
  {
    CreateOrReplaceTalentPayload payload = new(" Protection ")
    {
      Tier = 2,
      Description = "  Lorsque le personnage porte un bouclier moyen ou lourd, il peut utiliser sa réaction afin d’ajouter à un jet de sauvegarde d’_Acrobaties_ le bonus de Défense conféré par son bouclier. Lorsqu’il en fait ainsi en portant un bouclier lourd afin de réduire de moitié les points de dégâts reçus, il ne reçoit aucun point de dégâts s’il réussit le jet de sauvegarde, ou réduit de moitié les points de dégâts reçus s’il l’échoue.  ",
      RequiredTalentId = _coupDeBouclier.EntityId
    };

    CreateOrReplaceTalentCommand command = new(Guid.NewGuid(), payload, Version: null);
    CreateOrReplaceTalentResult result = await Pipeline.ExecuteAsync(command);
    Assert.True(result.Created);

    TalentModel? talent = result.Talent;
    Assert.NotNull(talent);
    Assert.Equal(command.Id, talent.Id);
    Assert.Equal(2, talent.Version);
    Assert.Equal(DateTime.UtcNow, talent.CreatedOn, TimeSpan.FromSeconds(1));
    Assert.True(talent.CreatedOn < talent.UpdatedOn);
    Assert.Equal(Actor, talent.CreatedBy);
    Assert.Equal(talent.CreatedBy, talent.UpdatedBy);

    Assert.Equal(World.Id.ToGuid(), talent.World.Id);

    Assert.Equal(payload.Name.Trim(), talent.Name);
    Assert.Equal(payload.Description?.CleanTrim(), talent.Description);

    Assert.Equal(payload.AllowMultiplePurchases, talent.AllowMultiplePurchases);
    Assert.Null(talent.Skill);
    //Assert.Equal(_coupDeBouclier.EntityId, talent.RequiredTalent?.Id); // TODO(fpion): complete talent unit tests

    Assert.NotNull(await SkillCraftContext.Talents.AsNoTracking().SingleOrDefaultAsync(x => x.Id == talent.Id));
  }

  [Fact(DisplayName = "It should replace an existing talent.")]
  public async Task It_should_replace_an_existing_talent()
  {
    long version = _manœuvresDeCombat.Version;

    Description description = new("  Le personnage apprend trois manœuvres de combat parmi une liste de manœuvres. Cette liste est définie dans les ouvrages spécifiques aux univers.  ");
    _manœuvresDeCombat.Description = description;
    _manœuvresDeCombat.Update(UserId);
    await _talentRepository.SaveAsync(_manœuvresDeCombat);

    CreateOrReplaceTalentPayload payload = new(" Manœuvres de combat ")
    {
      Tier = 2,
      Description = "    ",
      AllowMultiplePurchases = true,
      RequiredTalentId = _formationMartiale.EntityId
    };

    CreateOrReplaceTalentCommand command = new(_manœuvresDeCombat.EntityId, payload, version);
    CreateOrReplaceTalentResult result = await Pipeline.ExecuteAsync(command);
    Assert.False(result.Created);

    TalentModel? talent = result.Talent;
    Assert.NotNull(talent);
    Assert.Equal(command.Id, talent.Id);
    Assert.Equal(_manœuvresDeCombat.Version + 1, talent.Version);
    Assert.Equal(DateTime.UtcNow, talent.UpdatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, talent.UpdatedBy);

    Assert.Equal(payload.Name.Trim(), talent.Name);
    Assert.Equal(description.Value, talent.Description);

    Assert.Equal(payload.AllowMultiplePurchases, talent.AllowMultiplePurchases);
    Assert.Null(talent.Skill);
    //Assert.Equal(_formationMartiale.EntityId, talent.RequiredTalent?.Id); // TODO(fpion): complete talent unit tests
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task It_should_return_empty_search_results()
  {
    SearchTalentsPayload payload = new();
    payload.Search.Terms.Add(new SearchTerm("test"));

    SearchTalentsQuery query = new(payload);
    SearchResults<TalentModel> results = await Pipeline.ExecuteAsync(query);

    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    SearchTalentsPayload payload = new()
    {
      AllowMultiplePurchases = false,
      RequiredTalentId = _melee.EntityId,
      Tier = new TierFilter("eq", [1]),
      Limit = 1
    };
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.Add(new SearchTerm("%a%"));
    payload.Search.Terms.Add(new SearchTerm("%m%"));
    payload.Sort.Add(new TalentSortOption(TalentSort.Name, isDescending: true));

    payload.Ids.Add(Guid.Empty);
    payload.Ids.AddRange((await _talentRepository.LoadAsync()).Select(talent => talent.EntityId));
    payload.Ids.Remove(_attaqueNeutralisante.EntityId);

    SearchTalentsQuery query = new(payload);
    SearchResults<TalentModel> results = await Pipeline.ExecuteAsync(query);

    Assert.Equal(1, results.Total);
    TalentModel talent = Assert.Single(results.Items);
    Assert.Equal(_charge.EntityId, talent.Id);
  }

  [Fact(DisplayName = "It should return the talent found by ID.")]
  public async Task It_should_return_the_talent_found_by_Id()
  {
    ReadTalentQuery query = new(_formationMartiale.EntityId);
    TalentModel? talent = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(talent);
    Assert.Equal(_formationMartiale.EntityId, talent.Id);
  }

  [Fact(DisplayName = "It should update an existing talent.")]
  public async Task It_should_update_an_existing_talent()
  {
    UpdateTalentPayload payload = new()
    {
      Description = new Change<string>("  Le personnage apprend trois manœuvres de combat parmi une liste de manœuvres. Cette liste est définie dans les ouvrages spécifiques aux univers.  "),
      RequiredTalentId = new Change<Guid?>(_formationMartiale.EntityId)
    };

    UpdateTalentCommand command = new(_manœuvresDeCombat.EntityId, payload);
    TalentModel? talent = await Pipeline.ExecuteAsync(command);

    Assert.NotNull(talent);
    Assert.Equal(command.Id, talent.Id);
    Assert.Equal(_manœuvresDeCombat.Version + 1, talent.Version);
    Assert.Equal(DateTime.UtcNow, talent.UpdatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, talent.UpdatedBy);

    Assert.Equal(_manœuvresDeCombat.Tier, talent.Tier);
    Assert.Equal(_manœuvresDeCombat.Name.Value, talent.Name);
    Assert.NotNull(payload.Description.Value);
    Assert.Equal(payload.Description.Value.CleanTrim(), talent.Description);

    Assert.Equal(_manœuvresDeCombat.AllowMultiplePurchases, talent.AllowMultiplePurchases);
    Assert.Equal(_manœuvresDeCombat.Skill, talent.Skill);
    //Assert.Equal(_formationMartiale.EntityId, talent.RequiredTalent?.Id); // TODO(fpion): complete talent unit tests
  }
}
