using Logitar;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Application.Personalities.Commands;
using SkillCraft.Application.Personalities.Queries;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Contracts.Personalities;
using SkillCraft.Domain;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Personalities;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Application.Personalities;

[Trait(Traits.Category, Categories.Integration)]
public class PersonalityTests : IntegrationTests
{
  private readonly ICustomizationRepository _customizationRepository;
  private readonly IPersonalityRepository _personalityRepository;

  private readonly Customization _feroce;
  private readonly Customization _infatigable;
  private readonly Personality _agile;
  private readonly Personality _bienveillant;
  private readonly Personality _muscle;
  private readonly Personality _stoique;

  public PersonalityTests() : base()
  {
    _customizationRepository = ServiceProvider.GetRequiredService<ICustomizationRepository>();
    _personalityRepository = ServiceProvider.GetRequiredService<IPersonalityRepository>();

    _feroce = new(World.Id, CustomizationType.Gift, new Name("Féroce"), UserId);
    _infatigable = new(World.Id, CustomizationType.Gift, new Name("Infatigable"), UserId);
    _agile = new(World.Id, new Name("Agile"), World.OwnerId);
    _bienveillant = new(World.Id, new Name("Bienveillant"), World.OwnerId);
    _muscle = new(World.Id, new Name("Musclé"), World.OwnerId);
    _stoique = new(World.Id, new Name("stoique"), World.OwnerId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _customizationRepository.SaveAsync([_feroce, _infatigable]);
    await _personalityRepository.SaveAsync([_agile, _bienveillant, _muscle, _stoique]);
  }

  [Fact(DisplayName = "It should create a new personality.")]
  public async Task It_should_create_a_new_personality()
  {
    CreateOrReplacePersonalityPayload payload = new(" Courroucé ")
    {
      Description = "  Les émotions du personnage sont vives et ses mouvements sont brusques.  ",
      Attribute = Attribute.Agility,
      GiftId = _feroce.EntityId
    };

    CreateOrReplacePersonalityCommand command = new(Guid.NewGuid(), payload, Version: null);
    CreateOrReplacePersonalityResult result = await Pipeline.ExecuteAsync(command);
    Assert.True(result.Created);

    PersonalityModel? personality = result.Personality;
    Assert.NotNull(personality);
    Assert.Equal(command.Id, personality.Id);
    Assert.Equal(2, personality.Version);
    Assert.Equal(DateTime.UtcNow, personality.CreatedOn, TimeSpan.FromSeconds(1));
    Assert.True(personality.CreatedOn < personality.UpdatedOn);
    Assert.Equal(Actor, personality.CreatedBy);
    Assert.Equal(personality.CreatedBy, personality.UpdatedBy);

    Assert.Equal(World.Id.ToGuid(), personality.World.Id);

    Assert.Equal(payload.Name.Trim(), personality.Name);
    Assert.Equal(payload.Description?.CleanTrim(), personality.Description);

    Assert.Equal(payload.Attribute, personality.Attribute);
    Assert.Equal(payload.GiftId, personality.Gift?.Id);

    Assert.NotNull(await SkillCraftContext.Personalities.AsNoTracking().SingleOrDefaultAsync(x => x.Id == personality.Id));
  }

  [Fact(DisplayName = "It should replace an existing personality.")]
  public async Task It_should_replace_an_existing_personality()
  {
    long version = _stoique.Version;

    Description description = new("La volonté du personnage est ferme, il est impassible et indifférent à a douleur.");
    _stoique.Description = description;
    _stoique.Update(UserId);
    await _personalityRepository.SaveAsync(_stoique);

    CreateOrReplacePersonalityPayload payload = new(" Stoïque ")
    {
      Description = "    ",
      Attribute = Attribute.Agility,
      GiftId = _infatigable.EntityId
    };

    CreateOrReplacePersonalityCommand command = new(_stoique.EntityId, payload, version);
    CreateOrReplacePersonalityResult result = await Pipeline.ExecuteAsync(command);
    Assert.False(result.Created);

    PersonalityModel? personality = result.Personality;
    Assert.NotNull(personality);
    Assert.Equal(command.Id, personality.Id);
    Assert.Equal(_stoique.Version + 1, personality.Version);
    Assert.Equal(DateTime.UtcNow, personality.UpdatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, personality.UpdatedBy);

    Assert.Equal(payload.Name.Trim(), personality.Name);
    Assert.Equal(description.Value, personality.Description);

    Assert.Equal(payload.Attribute, personality.Attribute);
    Assert.Equal(payload.GiftId, personality.Gift?.Id);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task It_should_return_empty_search_results()
  {
    SearchPersonalitiesPayload payload = new();
    payload.Search.Terms.Add(new SearchTerm("test"));

    SearchPersonalitiesQuery query = new(payload);
    SearchResults<PersonalityModel> results = await Pipeline.ExecuteAsync(query);

    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    SearchPersonalitiesPayload payload = new()
    {
      Skip = 1,
      Limit = 1
    };
    payload.Search.Terms.Add(new SearchTerm("%i%"));
    payload.Sort.Add(new PersonalitySortOption(PersonalitySort.Name, isDescending: false));

    payload.Ids.Add(Guid.Empty);
    payload.Ids.AddRange((await _personalityRepository.LoadAsync()).Select(personality => personality.EntityId));
    payload.Ids.Remove(_stoique.EntityId);

    SearchPersonalitiesQuery query = new(payload);
    SearchResults<PersonalityModel> results = await Pipeline.ExecuteAsync(query);

    Assert.Equal(2, results.Total);
    PersonalityModel personality = Assert.Single(results.Items);
    Assert.Equal(_bienveillant.EntityId, personality.Id);
  }

  [Fact(DisplayName = "It should return the personality found by ID.")]
  public async Task It_should_return_the_personality_found_by_Id()
  {
    ReadPersonalityQuery query = new(_agile.EntityId);
    PersonalityModel? personality = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(personality);
    Assert.Equal(_agile.EntityId, personality.Id);
  }

  [Fact(DisplayName = "It should update an existing personality.")]
  public async Task It_should_update_an_existing_personality()
  {
    UpdatePersonalityPayload payload = new()
    {
      Description = new Change<string>("  La musculature du personnage est développée, vive et efficace.  "),
      Attribute = new Change<Attribute?>(Attribute.Vigor)
    };

    UpdatePersonalityCommand command = new(_muscle.EntityId, payload);
    PersonalityModel? personality = await Pipeline.ExecuteAsync(command);

    Assert.NotNull(personality);
    Assert.Equal(command.Id, personality.Id);
    Assert.Equal(_muscle.Version + 1, personality.Version);
    Assert.Equal(DateTime.UtcNow, personality.UpdatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, personality.UpdatedBy);

    Assert.Equal(_muscle.Name.Value, personality.Name);
    Assert.NotNull(payload.Description.Value);
    Assert.Equal(payload.Description.Value.CleanTrim(), personality.Description);
    Assert.Equal(payload.Attribute.Value, personality.Attribute);
    Assert.Equal(_muscle.GiftId?.EntityId, personality.Gift?.Id);
  }
}
