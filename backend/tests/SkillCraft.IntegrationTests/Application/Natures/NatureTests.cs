using Logitar;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Application.Natures.Commands;
using SkillCraft.Application.Natures.Queries;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Contracts.Natures;
using SkillCraft.Domain;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Natures;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Application.Natures;

[Trait(Traits.Category, Categories.Integration)]
public class NatureTests : IntegrationTests
{
  private readonly ICustomizationRepository _customizationRepository;
  private readonly INatureRepository _natureRepository;

  private readonly Customization _feroce;
  private readonly Customization _infatigable;
  private readonly Nature _agile;
  private readonly Nature _bienveillant;
  private readonly Nature _muscle;
  private readonly Nature _stoique;

  public NatureTests() : base()
  {
    _customizationRepository = ServiceProvider.GetRequiredService<ICustomizationRepository>();
    _natureRepository = ServiceProvider.GetRequiredService<INatureRepository>();

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
    await _natureRepository.SaveAsync([_agile, _bienveillant, _muscle, _stoique]);
  }

  [Fact(DisplayName = "It should create a new nature.")]
  public async Task It_should_create_a_new_nature()
  {
    CreateOrReplaceNaturePayload payload = new(" Courroucé ")
    {
      Description = "  Les émotions du personnage sont vives et ses mouvements sont brusques.  ",
      Attribute = Attribute.Agility,
      GiftId = _feroce.EntityId
    };

    CreateOrReplaceNatureCommand command = new(Guid.NewGuid(), payload, Version: null);
    CreateOrReplaceNatureResult result = await Pipeline.ExecuteAsync(command);
    Assert.True(result.Created);

    NatureModel? nature = result.Nature;
    Assert.NotNull(nature);
    Assert.Equal(command.Id, nature.Id);
    Assert.Equal(2, nature.Version);
    Assert.Equal(DateTime.UtcNow, nature.CreatedOn, TimeSpan.FromSeconds(1));
    Assert.True(nature.CreatedOn < nature.UpdatedOn);
    Assert.Equal(Actor, nature.CreatedBy);
    Assert.Equal(nature.CreatedBy, nature.UpdatedBy);

    Assert.Equal(World.Id.ToGuid(), nature.World.Id);

    Assert.Equal(payload.Name.Trim(), nature.Name);
    Assert.Equal(payload.Description?.CleanTrim(), nature.Description);

    Assert.Equal(payload.Attribute, nature.Attribute);
    Assert.Equal(payload.GiftId, nature.Gift?.Id);

    Assert.NotNull(await SkillCraftContext.Natures.AsNoTracking().SingleOrDefaultAsync(x => x.Id == nature.Id));
  }

  [Fact(DisplayName = "It should replace an existing nature.")]
  public async Task It_should_replace_an_existing_nature()
  {
    long version = _stoique.Version;

    Description description = new("La volonté du personnage est ferme, il est impassible et indifférent à a douleur.");
    _stoique.Description = description;
    _stoique.Update(UserId);
    await _natureRepository.SaveAsync(_stoique);

    CreateOrReplaceNaturePayload payload = new(" Stoïque ")
    {
      Description = "    ",
      Attribute = Attribute.Agility,
      GiftId = _infatigable.EntityId
    };

    CreateOrReplaceNatureCommand command = new(_stoique.EntityId, payload, version);
    CreateOrReplaceNatureResult result = await Pipeline.ExecuteAsync(command);
    Assert.False(result.Created);

    NatureModel? nature = result.Nature;
    Assert.NotNull(nature);
    Assert.Equal(command.Id, nature.Id);
    Assert.Equal(_stoique.Version + 1, nature.Version);
    Assert.Equal(DateTime.UtcNow, nature.UpdatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, nature.UpdatedBy);

    Assert.Equal(payload.Name.Trim(), nature.Name);
    Assert.Equal(description.Value, nature.Description);

    Assert.Equal(payload.Attribute, nature.Attribute);
    Assert.Equal(payload.GiftId, nature.Gift?.Id);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task It_should_return_empty_search_results()
  {
    SearchNaturesPayload payload = new();
    payload.Search.Terms.Add(new SearchTerm("test"));

    SearchNaturesQuery query = new(payload);
    SearchResults<NatureModel> results = await Pipeline.ExecuteAsync(query);

    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    SearchNaturesPayload payload = new()
    {
      Skip = 1,
      Limit = 1
    };
    payload.Search.Terms.Add(new SearchTerm("%i%"));
    payload.Sort.Add(new NatureSortOption(NatureSort.Name, isDescending: false));

    payload.Ids.Add(Guid.Empty);
    payload.Ids.AddRange((await _natureRepository.LoadAsync()).Select(nature => nature.EntityId));
    payload.Ids.Remove(_stoique.EntityId);

    SearchNaturesQuery query = new(payload);
    SearchResults<NatureModel> results = await Pipeline.ExecuteAsync(query);

    Assert.Equal(2, results.Total);
    NatureModel nature = Assert.Single(results.Items);
    Assert.Equal(_bienveillant.EntityId, nature.Id);
  }

  [Fact(DisplayName = "It should return the nature found by ID.")]
  public async Task It_should_return_the_nature_found_by_Id()
  {
    ReadNatureQuery query = new(_agile.EntityId);
    NatureModel? nature = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(nature);
    Assert.Equal(_agile.EntityId, nature.Id);
  }

  [Fact(DisplayName = "It should update an existing nature.")]
  public async Task It_should_update_an_existing_nature()
  {
    UpdateNaturePayload payload = new()
    {
      Description = new Change<string>("  La musculature du personnage est développée, vive et efficace.  "),
      Attribute = new Change<Attribute?>(Attribute.Vigor)
    };

    UpdateNatureCommand command = new(_muscle.EntityId, payload);
    NatureModel? nature = await Pipeline.ExecuteAsync(command);

    Assert.NotNull(nature);
    Assert.Equal(command.Id, nature.Id);
    Assert.Equal(_muscle.Version + 1, nature.Version);
    Assert.Equal(DateTime.UtcNow, nature.UpdatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, nature.UpdatedBy);

    Assert.Equal(_muscle.Name.Value, nature.Name);
    Assert.NotNull(payload.Description.Value);
    Assert.Equal(payload.Description.Value.CleanTrim(), nature.Description);
    Assert.Equal(payload.Attribute.Value, nature.Attribute);
    Assert.Equal(_muscle.GiftId?.EntityId, nature.Gift?.Id);
  }
}
