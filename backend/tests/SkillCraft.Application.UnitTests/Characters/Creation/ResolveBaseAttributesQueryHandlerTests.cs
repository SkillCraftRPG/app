using SkillCraft.Contracts.Characters;
using SkillCraft.Domain;
using SkillCraft.Domain.Aspects;
using SkillCraft.Domain.Characters;
using SkillCraft.Domain.Lineages;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Application.Characters.Creation;

[Trait(Traits.Category, Categories.Unit)]
public class ResolveBaseAttributesQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly ResolveBaseAttributesQueryHandler _handler = new();

  private readonly WorldMock _world = new();
  private readonly Aspect[] _aspects;
  private readonly Lineage _species;
  private readonly Lineage _nation;

  public ResolveBaseAttributesQueryHandlerTests()
  {
    _aspects =
    [
      new(_world.Id, new Name("Farouche"), _world.OwnerId)
      {
        Attributes = new AttributeSelection(mandatory1: Attribute.Agility, mandatory2: Attribute.Sensitivity, optional1: Attribute.Spirit, optional2: Attribute.Vigor)
      },
      new(_world.Id, new Name("Gymnaste"), _world.OwnerId)
      {
        Attributes = new AttributeSelection(mandatory1: Attribute.Agility, mandatory2: Attribute.Vigor, optional1: Attribute.Coordination, optional2: Attribute.Sensitivity)
      }
    ];
    _species = new(_world.Id, parent: null, new Name("Species"), _world.OwnerId)
    {
      Attributes = new AttributeBonuses(agility: 0, coordination: 0, intellect: 0, presence: 0, sensitivity: 0, spirit: 0, vigor: 0, extra: 2)
    };
    _nation = new(_world.Id, _species, new Name("Nation"), _world.OwnerId)
    {
      Attributes = new AttributeBonuses(agility: 0, coordination: 0, intellect: 0, presence: 0, sensitivity: 0, spirit: 0, vigor: 0, extra: 1)
    };
  }

  [Fact(DisplayName = "It should return the correct base attributes.")]
  public async Task It_should_return_the_correct_base_attributes()
  {
    BaseAttributesPayload payload = new()
    {
      Agility = 9,
      Coordination = 9,
      Intellect = 6,
      Presence = 10,
      Sensitivity = 7,
      Spirit = 6,
      Vigor = 10,
      Best = Attribute.Agility,
      Worst = Attribute.Sensitivity,
      Optional = [Attribute.Coordination, Attribute.Vigor],
      Extra = [Attribute.Agility, Attribute.Coordination, Attribute.Vigor],
    };
    ResolveBaseAttributesQuery query = new(payload, _aspects, _nation, _species);

    BaseAttributes baseAttributes = await _handler.Handle(query, _cancellationToken);
    Assert.Equal(payload.Agility, baseAttributes.Agility);
    Assert.Equal(payload.Coordination, baseAttributes.Coordination);
    Assert.Equal(payload.Intellect, baseAttributes.Intellect);
    Assert.Equal(payload.Presence, baseAttributes.Presence);
    Assert.Equal(payload.Sensitivity, baseAttributes.Sensitivity);
    Assert.Equal(payload.Spirit, baseAttributes.Spirit);
    Assert.Equal(payload.Vigor, baseAttributes.Vigor);
    Assert.Equal(payload.Best, baseAttributes.Best);
    Assert.Equal(payload.Worst, baseAttributes.Worst);
    Assert.Equal(payload.Optional, baseAttributes.Optional);
    Assert.Equal(payload.Extra, baseAttributes.Extra);
  }

  [Fact(DisplayName = "It should throw InvalidAspectAttributeSelectionException when the best attribute is not in the selection.")]
  public async Task It_should_throw_InvalidAspectAttributeSelectionException_when_the_best_attribute_is_not_in_the_selection()
  {
    BaseAttributesPayload payload = new()
    {
      Best = Attribute.Coordination,
      Worst = Attribute.Sensitivity
    };
    ResolveBaseAttributesQuery query = new(payload, _aspects, _nation, _species);

    var exception = await Assert.ThrowsAsync<InvalidAspectAttributeSelectionException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(payload.Best, exception.Attribute);
    Assert.Equal("Best", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw InvalidAspectAttributeSelectionException when the first optional attribute is not in the selection.")]
  public async Task It_should_throw_InvalidAspectAttributeSelectionException_when_the_first_optional_attribute_is_not_in_the_selection()
  {
    BaseAttributesPayload payload = new()
    {
      Best = Attribute.Agility,
      Worst = Attribute.Sensitivity,
      Optional = [Attribute.Intellect, Attribute.Sensitivity]
    };
    ResolveBaseAttributesQuery query = new(payload, _aspects, _nation, _species);

    var exception = await Assert.ThrowsAsync<InvalidAspectAttributeSelectionException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(payload.Optional[0], exception.Attribute);
    Assert.Equal("Optional[0]", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw InvalidAspectAttributeSelectionException when the second optional attribute is not in the selection.")]
  public async Task It_should_throw_InvalidAspectAttributeSelectionException_when_the_second_optional_attribute_is_not_in_the_selection()
  {
    BaseAttributesPayload payload = new()
    {
      Best = Attribute.Agility,
      Worst = Attribute.Sensitivity,
      Optional = [Attribute.Vigor, Attribute.Presence]
    };
    ResolveBaseAttributesQuery query = new(payload, _aspects, _nation, _species);

    var exception = await Assert.ThrowsAsync<InvalidAspectAttributeSelectionException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(payload.Optional[1], exception.Attribute);
    Assert.Equal("Optional[1]", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw InvalidAspectAttributeSelectionException when the worst attribute is not in the selection.")]
  public async Task It_should_throw_InvalidAspectAttributeSelectionException_when_the_worst_attribute_is_not_in_the_selection()
  {
    BaseAttributesPayload payload = new()
    {
      Best = Attribute.Agility,
      Worst = Attribute.Spirit
    };
    ResolveBaseAttributesQuery query = new(payload, _aspects, _nation, _species);

    var exception = await Assert.ThrowsAsync<InvalidAspectAttributeSelectionException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(payload.Worst, exception.Attribute);
    Assert.Equal("Worst", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw InvalidExtraAttributesException when the extra attribute count did not match the expected.")]
  public async Task It_should_throw_InvalidExtraAttributesException_when_the_extra_attribute_count_did_not_match_the_expected()
  {
    BaseAttributesPayload payload = new()
    {
      Best = Attribute.Agility,
      Worst = Attribute.Sensitivity,
      Extra = [Attribute.Agility, Attribute.Agility, Attribute.Vigor]
    };
    ResolveBaseAttributesQuery query = new(payload, _aspects, _nation, _species);

    var exception = await Assert.ThrowsAsync<InvalidExtraAttributesException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(payload.Extra, exception.ExtraAttributes);
    Assert.Equal(_species.Attributes.Extra + _nation.Attributes.Extra, exception.ExpectedCount);
    Assert.Equal("Extra", exception.PropertyName);
  }
}
