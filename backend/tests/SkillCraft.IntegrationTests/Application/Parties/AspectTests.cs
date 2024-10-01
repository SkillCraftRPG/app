using Logitar;
using Logitar.Portal.Contracts.Search;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Application.Parties.Commands;
using SkillCraft.Application.Parties.Queries;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Parties;
using SkillCraft.Domain;
using SkillCraft.Domain.Parties;

namespace SkillCraft.Application.Parties;

[Trait(Traits.Category, Categories.Unit)]
public class PartyTests : IntegrationTests
{
  private readonly IPartyRepository _partyRepository;

  private readonly Party _ascension;
  private readonly Party _brigadeArdente;
  private readonly Party _confrerieMystique;
  private readonly Party _griffeNoire;

  public PartyTests() : base()
  {
    _partyRepository = ServiceProvider.GetRequiredService<IPartyRepository>();

    _ascension = new(World.Id, new Name("Ascension"), World.OwnerId);
    _brigadeArdente = new(World.Id, new Name("Brigade Ardente"), World.OwnerId);
    _confrerieMystique = new(World.Id, new Name("Confrérie Mystique"), World.OwnerId);
    _griffeNoire = new(World.Id, new Name("Griffe Noire"), World.OwnerId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _partyRepository.SaveAsync([_ascension, _brigadeArdente, _confrerieMystique, _griffeNoire]);
  }

  //[Fact(DisplayName = "It should create a new party.")]
  //public async Task It_should_create_a_new_party()
  //{
  //  SavePartyPayload payload = new(" Gymnaste ")
  //  {
  //    Description = "    ",
  //    Attributes = new AttributeSelectionModel
  //    {
  //      Mandatory1 = Attribute.Agility,
  //      Mandatory2 = Attribute.Vigor,
  //      Optional1 = Attribute.Coordination,
  //      Optional2 = Attribute.Sensitivity
  //    },
  //    Skills = new SkillsModel
  //    {
  //      Discounted1 = Skill.Acrobatics,
  //      Discounted2 = Skill.Athletics
  //    }
  //  };

  //  SavePartyCommand command = new(Guid.NewGuid(), payload, Version: null);
  //  SavePartyResult result = await Pipeline.ExecuteAsync(command);
  //  Assert.True(result.Created);

  //  PartyModel? party = result.Party;
  //  Assert.NotNull(party);
  //  Assert.Equal(command.Id, party.Id);
  //  Assert.Equal(2, party.Version);
  //  Assert.Equal(DateTime.UtcNow, party.CreatedOn, TimeSpan.FromSeconds(1));
  //  Assert.True(party.CreatedOn < party.UpdatedOn);
  //  Assert.Equal(Actor, party.CreatedBy);
  //  Assert.Equal(party.CreatedBy, party.UpdatedBy);

  //  Assert.Equal(World.Id.ToGuid(), party.World.Id);

  //  Assert.Equal(payload.Name.Trim(), party.Name);
  //  Assert.Equal(payload.Description?.CleanTrim(), party.Description);

  //  Assert.Equal(payload.Attributes, party.Attributes);
  //  Assert.Equal(payload.Skills, party.Skills);

  //  Assert.NotNull(await SkillCraftContext.Parties.AsNoTracking().SingleOrDefaultAsync(x => x.Id == party.Id));
  //}

  //[Fact(DisplayName = "It should replace an existing party.")]
  //public async Task It_should_replace_an_existing_party()
  //{
  //  long version = _tenace.Version;

  //  Description description = new("Personne qui pratique la gymnastique sportive.");
  //  _tenace.Description = description;
  //  _tenace.Update(UserId);
  //  await _partyRepository.SaveAsync(_tenace);

  //  SavePartyPayload payload = new(" Gymnaste ")
  //  {
  //    Description = "    ",
  //    Attributes = new AttributeSelectionModel
  //    {
  //      Mandatory1 = Attribute.Agility,
  //      Mandatory2 = Attribute.Vigor,
  //      Optional1 = Attribute.Coordination,
  //      Optional2 = Attribute.Sensitivity
  //    },
  //    Skills = new SkillsModel
  //    {
  //      Discounted1 = Skill.Acrobatics,
  //      Discounted2 = Skill.Athletics
  //    }
  //  };

  //  SavePartyCommand command = new(_tenace.EntityId, payload, version);
  //  SavePartyResult result = await Pipeline.ExecuteAsync(command);
  //  Assert.False(result.Created);

  //  PartyModel? party = result.Party;
  //  Assert.NotNull(party);
  //  Assert.Equal(command.Id, party.Id);
  //  Assert.Equal(_tenace.Version + 1, party.Version);
  //  Assert.Equal(DateTime.UtcNow, party.UpdatedOn, TimeSpan.FromSeconds(1));
  //  Assert.Equal(Actor, party.UpdatedBy);

  //  Assert.Equal(payload.Name.Trim(), party.Name);
  //  Assert.Equal(description.Value, party.Description);

  //  Assert.Equal(payload.Attributes, party.Attributes);
  //  Assert.Equal(payload.Skills, party.Skills);
  //}

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task It_should_return_empty_search_results()
  {
    SearchPartiesPayload payload = new();
    payload.Search.Terms.Add(new SearchTerm("test"));

    SearchPartiesQuery query = new(payload);
    SearchResults<PartyModel> results = await Pipeline.ExecuteAsync(query);

    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    SearchPartiesPayload payload = new()
    {
      Skip = 1,
      Limit = 1
    };
    payload.Search.Operator = SearchOperator.Or;
    payload.Search.Terms.Add(new SearchTerm("%a%"));
    payload.Search.Terms.Add(new SearchTerm("%g%"));
    payload.Sort.Add(new PartySortOption(PartySort.Name, isDescending: false));

    payload.Ids.Add(Guid.Empty);
    payload.Ids.AddRange((await _partyRepository.LoadAsync()).Select(party => party.EntityId));
    payload.Ids.Remove(_brigadeArdente.EntityId);

    SearchPartiesQuery query = new(payload);
    SearchResults<PartyModel> results = await Pipeline.ExecuteAsync(query);

    Assert.Equal(2, results.Total);
    PartyModel party = Assert.Single(results.Items);
    Assert.Equal(_griffeNoire.EntityId, party.Id);
  }

  [Fact(DisplayName = "It should return the party found by ID.")]
  public async Task It_should_return_the_party_found_by_Id()
  {
    ReadPartyQuery query = new(_confrerieMystique.EntityId);
    PartyModel? party = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(party);
    Assert.Equal(_confrerieMystique.EntityId, party.Id);
  }

  [Fact(DisplayName = "It should update an existing party.")]
  public async Task It_should_update_an_existing_party()
  {
    UpdatePartyPayload payload = new()
    {
      Description = new Change<string>("  Suivez trois jeunes héros dans leur ascension en divinité.  ")
    };

    UpdatePartyCommand command = new(_ascension.EntityId, payload);
    PartyModel? party = await Pipeline.ExecuteAsync(command);

    Assert.NotNull(party);
    Assert.Equal(command.Id, party.Id);
    Assert.Equal(_ascension.Version + 1, party.Version);
    Assert.Equal(DateTime.UtcNow, party.UpdatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, party.UpdatedBy);

    Assert.Equal(_ascension.Name.Value, party.Name);
    Assert.NotNull(payload.Description.Value);
    Assert.Equal(payload.Description.Value.CleanTrim(), party.Description);
  }
}
