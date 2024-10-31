using Logitar;
using Logitar.Portal.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Application.Aspects.Commands;
using SkillCraft.Application.Aspects.Queries;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Domain;
using SkillCraft.Domain.Aspects;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Application.Aspects;

[Trait(Traits.Category, Categories.Integration)]
public class AspectTests : IntegrationTests
{
  private readonly IAspectRepository _aspectRepository;

  private readonly Aspect _adroit;
  private readonly Aspect _charismatique;
  private readonly Aspect _circonspect;
  private readonly Aspect _farouche;
  private readonly Aspect _tenace;

  public AspectTests() : base()
  {
    _aspectRepository = ServiceProvider.GetRequiredService<IAspectRepository>();

    _adroit = new(World.Id, new Name("Adroit"), UserId)
    {
      Attributes = new(Attribute.Agility, Attribute.Coordination, Attribute.Spirit, Attribute.Vigor)
    };
    _adroit.Update(UserId);
    _charismatique = new(World.Id, new Name("Charismatique"), UserId)
    {
      Attributes = new(Attribute.Presence, Attribute.Sensitivity, Attribute.Agility, Attribute.Spirit)
    };
    _charismatique.Update(UserId);
    _circonspect = new(World.Id, new Name("Circonspect"), UserId)
    {
      Attributes = new(Attribute.Intellect, Attribute.Sensitivity, Attribute.Agility, Attribute.Coordination)
    };
    _circonspect.Update(UserId);
    _farouche = new(World.Id, new Name("Farouche"), UserId)
    {
      Attributes = new(Attribute.Agility, Attribute.Sensitivity, Attribute.Spirit, Attribute.Vigor)
    };
    _farouche.Update(UserId);
    _tenace = new(World.Id, new Name("Tenace"), UserId)
    {
      Attributes = new(Attribute.Spirit, Attribute.Presence, Attribute.Intellect, Attribute.Sensitivity)
    };
    _tenace.Update(UserId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _aspectRepository.SaveAsync([_adroit, _charismatique, _circonspect, _farouche, _tenace]);
  }

  [Fact(DisplayName = "It should create a new aspect.")]
  public async Task It_should_create_a_new_aspect()
  {
    CreateOrReplaceAspectPayload payload = new(" Gymnaste ")
    {
      Description = "    ",
      Attributes = new AttributeSelectionModel
      {
        Mandatory1 = Attribute.Agility,
        Mandatory2 = Attribute.Vigor,
        Optional1 = Attribute.Coordination,
        Optional2 = Attribute.Sensitivity
      },
      Skills = new SkillsModel
      {
        Discounted1 = Skill.Acrobatics,
        Discounted2 = Skill.Athletics
      }
    };

    CreateOrReplaceAspectCommand command = new(Guid.NewGuid(), payload, Version: null);
    CreateOrReplaceAspectResult result = await Pipeline.ExecuteAsync(command);
    Assert.True(result.Created);

    AspectModel? aspect = result.Aspect;
    Assert.NotNull(aspect);
    Assert.Equal(command.Id, aspect.Id);
    Assert.Equal(2, aspect.Version);
    Assert.Equal(DateTime.UtcNow, aspect.CreatedOn, TimeSpan.FromSeconds(1));
    Assert.True(aspect.CreatedOn < aspect.UpdatedOn);
    Assert.Equal(Actor, aspect.CreatedBy);
    Assert.Equal(aspect.CreatedBy, aspect.UpdatedBy);

    Assert.Equal(World.Id.ToGuid(), aspect.World.Id);

    Assert.Equal(payload.Name.Trim(), aspect.Name);
    Assert.Equal(payload.Description?.CleanTrim(), aspect.Description);

    Assert.Equal(payload.Attributes, aspect.Attributes);
    Assert.Equal(payload.Skills, aspect.Skills);

    Assert.NotNull(await SkillCraftContext.Aspects.AsNoTracking().SingleOrDefaultAsync(x => x.Id == aspect.Id));
  }

  [Fact(DisplayName = "It should replace an existing aspect.")]
  public async Task It_should_replace_an_existing_aspect()
  {
    long version = _tenace.Version;

    Description description = new("Personne qui pratique la gymnastique sportive.");
    _tenace.Description = description;
    _tenace.Update(UserId);
    await _aspectRepository.SaveAsync(_tenace);

    CreateOrReplaceAspectPayload payload = new(" Gymnaste ")
    {
      Description = "    ",
      Attributes = new AttributeSelectionModel
      {
        Mandatory1 = Attribute.Agility,
        Mandatory2 = Attribute.Vigor,
        Optional1 = Attribute.Coordination,
        Optional2 = Attribute.Sensitivity
      },
      Skills = new SkillsModel
      {
        Discounted1 = Skill.Acrobatics,
        Discounted2 = Skill.Athletics
      }
    };

    CreateOrReplaceAspectCommand command = new(_tenace.EntityId, payload, version);
    CreateOrReplaceAspectResult result = await Pipeline.ExecuteAsync(command);
    Assert.False(result.Created);

    AspectModel? aspect = result.Aspect;
    Assert.NotNull(aspect);
    Assert.Equal(command.Id, aspect.Id);
    Assert.Equal(_tenace.Version + 1, aspect.Version);
    Assert.Equal(DateTime.UtcNow, aspect.UpdatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, aspect.UpdatedBy);

    Assert.Equal(payload.Name.Trim(), aspect.Name);
    Assert.Equal(description.Value, aspect.Description);

    Assert.Equal(payload.Attributes, aspect.Attributes);
    Assert.Equal(payload.Skills, aspect.Skills);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task It_should_return_empty_search_results()
  {
    SearchAspectsPayload payload = new();
    payload.Search.Terms.Add(new SearchTerm("test"));

    SearchAspectsQuery query = new(payload);
    SearchResults<AspectModel> results = await Pipeline.ExecuteAsync(query);

    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    SearchAspectsPayload payload = new()
    {
      Attribute = Attribute.Agility,
      Skip = 1,
      Limit = 1
    };
    payload.Search.Terms.Add(new SearchTerm("%a%"));
    payload.Sort.Add(new AspectSortOption(AspectSort.Name, isDescending: false));

    payload.Ids.Add(Guid.Empty);
    payload.Ids.AddRange((await _aspectRepository.LoadAsync()).Select(aspect => aspect.EntityId));
    payload.Ids.Remove(_farouche.EntityId);

    SearchAspectsQuery query = new(payload);
    SearchResults<AspectModel> results = await Pipeline.ExecuteAsync(query);

    Assert.Equal(2, results.Total);
    AspectModel aspect = Assert.Single(results.Items);
    Assert.Equal(_charismatique.EntityId, aspect.Id);
  }

  [Fact(DisplayName = "It should return the aspect found by ID.")]
  public async Task It_should_return_the_aspect_found_by_Id()
  {
    ReadAspectQuery query = new(_adroit.EntityId);
    AspectModel? aspect = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(aspect);
    Assert.Equal(_adroit.EntityId, aspect.Id);
  }

  [Fact(DisplayName = "It should update an existing aspect.")]
  public async Task It_should_update_an_existing_aspect()
  {
    UpdateAspectPayload payload = new()
    {
      Description = new Change<string>("  Qui agit avec réflexion et prudence ou qui manifeste cela.  ")
    };

    UpdateAspectCommand command = new(_circonspect.EntityId, payload);
    AspectModel? aspect = await Pipeline.ExecuteAsync(command);

    Assert.NotNull(aspect);
    Assert.Equal(command.Id, aspect.Id);
    Assert.Equal(_circonspect.Version + 1, aspect.Version);
    Assert.Equal(DateTime.UtcNow, aspect.UpdatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(Actor, aspect.UpdatedBy);

    Assert.Equal(_circonspect.Name.Value, aspect.Name);
    Assert.NotNull(payload.Description.Value);
    Assert.Equal(payload.Description.Value.CleanTrim(), aspect.Description);

    Assert.Equal(_circonspect.Attributes.Mandatory1, aspect.Attributes.Mandatory1);
    Assert.Equal(_circonspect.Attributes.Mandatory2, aspect.Attributes.Mandatory2);
    Assert.Equal(_circonspect.Attributes.Optional1, aspect.Attributes.Optional1);
    Assert.Equal(_circonspect.Attributes.Optional2, aspect.Attributes.Optional2);
    Assert.Equal(_circonspect.Skills.Discounted1, aspect.Skills.Discounted1);
    Assert.Equal(_circonspect.Skills.Discounted2, aspect.Skills.Discounted2);
  }
}
