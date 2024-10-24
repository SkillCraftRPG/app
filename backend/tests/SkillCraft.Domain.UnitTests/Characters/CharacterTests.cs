﻿using SkillCraft.Contracts;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain.Aspects;
using SkillCraft.Domain.Castes;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Educations;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Personalities;
using SkillCraft.Domain.Talents;
using SkillCraft.Domain.Worlds;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Domain.Characters;

[Trait(Traits.Category, Categories.Unit)]
public class CharacterTests
{
  private readonly World _world = new(new Slug("ungar"), UserId.NewId());
  private readonly Lineage _lineage;
  private readonly Customization _customization;
  private readonly Personality _personality;
  private readonly Aspect[] _aspects;
  private readonly BaseAttributes _baseAttributes = new(agility: 9, coordination: 9, intellect: 6, presence: 10, sensitivity: 7, spirit: 6, vigor: 10,
    best: Attribute.Agility, worst: Attribute.Sensitivity, mandatory: [Attribute.Agility, Attribute.Vigor], optional: [Attribute.Coordination, Attribute.Vigor],
    extra: [Attribute.Agility, Attribute.Vigor]);
  private readonly Caste _caste;
  private readonly Education _education;
  private readonly Language _language;
  private readonly Talent _talent;

  private readonly Character _character;

  public CharacterTests()
  {
    _lineage = new(_world.Id, parent: null, new Name("Humain"), _world.OwnerId);
    _customization = new(_world.Id, CustomizationType.Gift, new Name("Féroce"), _world.OwnerId);
    _personality = new(_world.Id, new Name("Courroucé"), _world.OwnerId);
    _personality.SetGift(_customization);
    _personality.Update(_world.OwnerId);
    _aspects =
    [
      new(_world.Id, new Name("Farouche"), _world.OwnerId),
      new(_world.Id, new Name("Gymnaste"), _world.OwnerId)
    ];
    _caste = new(_world.Id, new Name("Milicien"), _world.OwnerId);
    _education = new(_world.Id, new Name("Champs de bataille"), _world.OwnerId);
    _language = new(_world.Id, new Name("Orrinique"), _world.OwnerId);
    _talent = new(_world.Id, tier: 0, new Name("Mêlée"), _world.OwnerId);

    _character = new(
      _world.Id,
      new Name("Heracles Aetos"),
      player: null,
      _lineage,
      height: 1.84,
      weight: 84.6,
      age: 30,
      _personality,
      customizations: [],
      _aspects,
      _baseAttributes,
      _caste,
      _education,
      _world.OwnerId);
  }

  [Fact(DisplayName = "It should throw ArgumentException when a customization is the same as the personality's gift.")]
  public void It_should_throw_ArgumentException_when_a_customization_is_the_same_as_the_personality_s_gift()
  {
    Customization[] customizations = [_customization];
    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84,
      weight: 84.6, age: 30, _personality, customizations, _aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.StartsWith("The customizations cannot include the same gift as the personality.", exception.Message);
    Assert.Equal("customizations", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when a customization resides in another world.")]
  public void It_should_throw_ArgumentException_when_a_customization_resides_in_another_world()
  {
    Customization[] customizations = [new(WorldId.NewId(), CustomizationType.Gift, new Name("Féroce"), UserId.NewId())];
    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84,
      weight: 84.6, age: 30, _personality, customizations, _aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.StartsWith("One or more customizations do not reside in the same world as the character.", exception.Message);
    Assert.Equal("customizations", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when an aspect resides in another world.")]
  public void It_should_throw_ArgumentException_when_an_aspect_resides_in_another_world()
  {
    Aspect[] aspects = [_aspects[0], new(WorldId.NewId(), new Name("Gymnaste"), UserId.NewId())];
    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84,
      weight: 84.6, age: 30, _personality, customizations: [], aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.StartsWith("One or more aspects do not reside in the same world as the character.", exception.Message);
    Assert.Equal("aspects", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when not exactly two different aspects were provided.")]
  public void It_should_throw_ArgumentException_when_not_exactly_two_different_aspects_were_provided()
  {
    Aspect[] aspects = [_aspects[0], _aspects[0]];
    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84,
      weight: 84.6, age: 30, _personality, customizations: [], aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.StartsWith("Exactly 2 different aspects should be provided.", exception.Message);
    Assert.Equal("aspects", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the caste resides in another world.")]
  public void It_should_throw_ArgumentException_when_the_caste_resides_in_another_world()
  {
    Caste caste = new(WorldId.NewId(), new Name("Amuseur"), UserId.NewId());

    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84,
      weight: 84.6, age: 30, _personality, customizations: [], _aspects, _baseAttributes, caste, _education, _world.OwnerId));
    Assert.StartsWith("The caste does not reside in the same world as the character.", exception.Message);
    Assert.Equal("caste", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the education resides in another world.")]
  public void It_should_throw_ArgumentException_when_the_education_resides_in_another_world()
  {
    Education education = new(WorldId.NewId(), new Name("Rebelle"), UserId.NewId());

    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84,
      weight: 84.6, age: 30, _personality, customizations: [], _aspects, _baseAttributes, _caste, education, _world.OwnerId));
    Assert.StartsWith("The education does not reside in the same world as the character.", exception.Message);
    Assert.Equal("education", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the lineage resides in another world.")]
  public void It_should_throw_ArgumentException_when_the_lineage_resides_in_another_world()
  {
    Lineage lineage = new(WorldId.NewId(), parent: null, new Name("Elfe"), UserId.NewId());

    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, lineage, height: 1.84,
      weight: 84.6, age: 30, _personality, customizations: [], _aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.StartsWith("The lineage does not reside in the same world as the character.", exception.Message);
    Assert.Equal("lineage", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the number of gifts does not equal the number of disabilities.")]
  public void It_should_throw_ArgumentException_when_the_number_of_gifts_does_not_equal_the_number_of_disabilities()
  {

    Customization[] customizations = [new(_world.Id, CustomizationType.Gift, new Name("Réflexes"), _world.OwnerId)];
    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84,
      weight: 84.6, age: 30, _personality, customizations, _aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.StartsWith("The customizations must contain an equal number of gifts and disabilities.", exception.Message);
    Assert.Equal("customizations", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when the personality resides in another world.")]
  public void It_should_throw_ArgumentException_when_the_personality_resides_in_another_world()
  {
    Personality personality = new(WorldId.NewId(), new Name("Courroucé"), UserId.NewId());

    var exception = Assert.Throws<ArgumentException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84,
      weight: 84.6, age: 30, personality, customizations: [], _aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.StartsWith("The personality does not reside in the same world as the character.", exception.Message);
    Assert.Equal("personality", exception.ParamName);
  }

  [Theory(DisplayName = "It should throw ArgumentOutOfRangeException when the age is not stricly positive.")]
  [InlineData(0)]
  [InlineData(-30)]
  public void It_should_throw_ArgumentOutOfRangeException_when_the_age_is_not_stricly_positive(int age)
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84,
      weight: 84.6, age, _personality, customizations: [], _aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.Equal("age", exception.ParamName);
  }

  [Theory(DisplayName = "It should throw ArgumentOutOfRangeException when the height is not stricly positive.")]
  [InlineData(0)]
  [InlineData(-1.84)]
  public void It_should_throw_ArgumentOutOfRangeException_when_the_height_is_not_stricly_positive(double height)
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height,
      weight: 84.6, age: 30, _personality, customizations: [], _aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.Equal("height", exception.ParamName);
  }

  [Theory(DisplayName = "It should throw ArgumentOutOfRangeException when the weight is not stricly positive.")]
  [InlineData(0)]
  [InlineData(-84.6)]
  public void It_should_throw_ArgumentOutOfRangeException_when_the_weight_is_not_stricly_positive(double weight)
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Character(_world.Id, new Name("Heracles Aetos"), player: null, _lineage, height: 1.84,
      weight, age: 30, _personality, customizations: [], _aspects, _baseAttributes, _caste, _education, _world.OwnerId));
    Assert.Equal("weight", exception.ParamName);
  }

  [Fact(DisplayName = "SetLanguage: it should add a new language.")]
  public void SetLanguage_it_should_add_a_new_language()
  {
    _character.SetLanguage(_language, notes: null, _world.OwnerId);
    Assert.Contains(_character.Languages, x => x.Key == _language.Id && x.Value.Notes == null);
  }

  [Fact(DisplayName = "SetLanguage: it should not do anything when the language metadata did not change.")]
  public void SetLanguage_it_should_not_do_anything_when_the_language_metadata_did_not_change()
  {
    _character.SetLanguage(_language, notes: null, _world.OwnerId);
    _character.ClearChanges();

    _character.SetLanguage(_language, notes: null, _world.OwnerId);
    Assert.False(_character.HasChanges);
    Assert.Empty(_character.Changes);
  }

  [Fact(DisplayName = "SetLanguage: it should replace an existing language.")]
  public void SetLanguage_it_should_replace_an_existing_language()
  {
    _character.SetLanguage(_language, notes: null, _world.OwnerId);

    Description notes = new("Lineage Extra Language");
    _character.SetLanguage(_language, notes, _world.OwnerId);
    Assert.Contains(_character.Languages, x => x.Key == _language.Id && x.Value.Notes == notes);
  }

  [Fact(DisplayName = "SetLanguage: it should throw ArgumentException when the language resides in another world.")]
  public void SetLanguage_it_should_throw_ArgumentException_when_the_language_resides_in_another_world()
  {
    UserId userId = UserId.NewId();
    Language language = new(WorldId.NewId(), new Name("Orrinique"), userId);

    var exception = Assert.Throws<ArgumentException>(() => _character.SetLanguage(language, notes: null, userId));
    Assert.StartsWith("The language does not reside in the same world as the character.", exception.Message);
    Assert.Equal("language", exception.ParamName);
  }

  [Fact(DisplayName = "SetTalent: it should add a new talent.")]
  public void SetTalent_it_should_add_a_new_talent()
  {
    _character.SetTalent(_talent, _world.OwnerId);

    KeyValuePair<TalentId, TalentMetadata> talent = Assert.Single(_character.Talents);
    Assert.Equal(_talent.Id, talent.Key);
    Assert.Equal(_talent.Tier + 2, talent.Value.Cost);
    Assert.Null(talent.Value.Precision);
    Assert.Null(talent.Value.Notes);
  }

  [Fact(DisplayName = "SetTalent: it should not do anything when the talent metadata did not change.")]
  public void SetTalent_it_should_not_do_anything_when_the_talent_metadata_did_not_change()
  {
    _character.SetTalent(_talent, _world.OwnerId);
    _character.ClearChanges();

    _character.SetTalent(_talent, _world.OwnerId);
    Assert.False(_character.HasChanges);
    Assert.Empty(_character.Changes);
  }

  [Fact(DisplayName = "SetTalent: it should replace an existing talent.")]
  public void SetTalent_it_should_replace_an_existing_talent()
  {
    _character.SetTalent(_talent, _world.OwnerId);

    SetTalentOptions options = new()
    {
      Cost = _talent.Tier + 2 - 1,
      Precision = new Name(Skill.Melee.ToString()),
      Notes = new Description("Caste: Milicien; Discounted: Farouche (aspect)")
    };
    _character.SetTalent(_talent, options, _world.OwnerId);

    KeyValuePair<TalentId, TalentMetadata> talent = Assert.Single(_character.Talents);
    Assert.Equal(_talent.Id, talent.Key);
    Assert.Equal(options.Cost, talent.Value.Cost);
    Assert.Equal(options.Precision, talent.Value.Precision);
    Assert.Equal(options.Notes, talent.Value.Notes);
  }

  [Fact(DisplayName = "SetTalent: it should throw ArgumentException when the talent resides in another world.")]
  public void SetTalent_it_should_throw_ArgumentException_when_the_talent_resides_in_another_world()
  {
    UserId userId = UserId.NewId();
    Talent talent = new(WorldId.NewId(), tier: 0, new Name("Mêlée"), userId);

    var exception = Assert.Throws<ArgumentException>(() => _character.SetTalent(talent, userId));
    Assert.StartsWith("The talent does not reside in the same world as the character.", exception.Message);
    Assert.Equal("talent", exception.ParamName);
  }

  [Fact(DisplayName = "SetTalent: it should throw ArgumentException when the talent cost is greater than the maximum cost.")]
  public void SetTalent_it_should_throw_ArgumentException_when_the_talent_tier_is_greater_than_the_maximum_cost()
  {
    SetTalentOptions options = new()
    {
      Cost = _talent.Tier + 2 + 1
    };
    var exception = Assert.Throws<ArgumentException>(() => _character.SetTalent(_talent, options, _world.OwnerId));
    Assert.StartsWith($"The cost cannot exceed the maximum cost (2) for the talent '{_talent}' of tier 0.", exception.Message);
    Assert.Equal("options", exception.ParamName);
  }

  [Fact(DisplayName = "SetTalent: it should throw ArgumentException when the talent tier is greater than the character tier.")]
  public void SetTalent_it_should_throw_ArgumentException_when_the_talent_tier_is_greater_than_the_character_tier()
  {
    Talent talent = new(_world.Id, tier: 2, new Name("Accélération"), _world.OwnerId);

    var exception = Assert.Throws<ArgumentException>(() => _character.SetTalent(talent, _world.OwnerId));
    Assert.StartsWith("The talent tier (2) cannot exceed the character tier (0).", exception.Message);
    Assert.Equal("talent", exception.ParamName);
  }
}
