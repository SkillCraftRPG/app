using SkillCraft.Domain.Talents;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Characters;

[Trait(Traits.Category, Categories.Unit)]
public class CharacterTalentTests
{
  private readonly WorldMock _world = new();

  private readonly Talent _melee;
  private readonly Talent _otherWorldTalent;
  private readonly Talent _formationMartiale;
  private readonly Talent _cuirasse;
  private readonly Talent _occultisme;
  private readonly Talent _elementarisme;

  private readonly Character _character;

  public CharacterTalentTests()
  {
    _melee = new(_world.Id, tier: 0, new Name("Mêlée"), _world.OwnerId);
    _otherWorldTalent = new(WorldId.NewId(), tier: 0, new Name("Mêlée"), UserId.NewId());

    _formationMartiale = new(_world.Id, tier: 0, new Name("Formation martiale"), _world.OwnerId);
    _formationMartiale.SetRequiredTalent(_melee);
    _formationMartiale.Update(_world.OwnerId);

    _cuirasse = new(_world.Id, tier: 1, new Name("Cuirassé"), _world.OwnerId);
    _cuirasse.SetRequiredTalent(_formationMartiale);
    _cuirasse.Update(_world.OwnerId);

    _occultisme = new(_world.Id, tier: 0, new Name("Occultisme"), _world.OwnerId);
    _elementarisme = new(_world.Id, tier: 0, new Name("Élémentarisme"), _world.OwnerId)
    {
      AllowMultiplePurchases = true
    };
    _elementarisme.SetRequiredTalent(_occultisme);
    _elementarisme.Update(_world.OwnerId);

    _character = new CharacterBuilder(_world).Build();
  }

  [Fact(DisplayName = "AddTalent: it should add a new talent with options.")]
  public void AddTalent_it_should_add_a_new_talent_with_options()
  {
    SetTalentOptions options = new()
    {
      Cost = 1,
      Precision = new Name("Melee"),
      Notes = new Description("Discounted by Aspect: Farouche")
    };
    _character.AddTalent(_melee, options, _world.OwnerId);

    CharacterTalent talent = Assert.Single(_character.Talents.Values);
    Assert.Equal(_melee.Id, talent.Id);
    Assert.Equal(options.Cost, talent.Cost);
    Assert.Equal(options.Precision, talent.Precision);
    Assert.Equal(options.Notes, talent.Notes);
  }

  [Fact(DisplayName = "AddTalent: it should add a new talent without options.")]
  public void AddTalent_it_should_add_a_new_talent_without_options()
  {
    _character.AddTalent(_melee, _world.OwnerId);

    CharacterTalent talent = Assert.Single(_character.Talents.Values);
    Assert.Equal(_melee.Id, talent.Id);
    Assert.Equal(_melee.Tier + 2, talent.Cost);
    Assert.Null(talent.Precision);
    Assert.Null(talent.Notes);
  }

  [Fact(DisplayName = "AddTalent: it should throw ArgumentException when the talent resides in another world.")]
  public void AddTalent_it_should_throw_ArgumentException_when_the_talent_resides_in_another_world()
  {
    var exception = Assert.Throws<ArgumentException>(() => _character.AddTalent(_otherWorldTalent, _world.OwnerId));
    Assert.StartsWith("The talent does not reside in the same world as the character.", exception.Message);
    Assert.Equal("talent", exception.ParamName);
  }

  [Fact(DisplayName = "AddTalent: it should throw ArgumentOutOfRangeException when the cost is negative.")]
  public void AddTalent_it_should_throw_ArgumentOutOfRangeException_when_the_cost_is_negative()
  {
    SetTalentOptions options = new()
    {
      Cost = -_melee.MaximumCost
    };
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _character.AddTalent(_melee, options, _world.OwnerId));
    Assert.StartsWith("The talent cost cannot be negative.", exception.Message);
    Assert.Equal("options", exception.ParamName);
  }

  [Fact(DisplayName = "AddTalent: it should throw NotEnoughRemainingTalentPointsException when there is not enough remaining talent points.")]
  public void AddTalent_it_should_throw_NotEnoughRemainingTalentPointsException_when_there_is_not_enough_remaining_talent_points()
  {
    _character.AddTalent(_melee, _world.OwnerId);
    _character.AddTalent(_formationMartiale, _world.OwnerId);
    _character.AddTalent(_occultisme, new SetTalentOptions
    {
      Cost = 1,
      Notes = new Description("Discounted by Aspect: Tenace")
    }, _world.OwnerId);
    _character.AddTalent(_elementarisme, new SetTalentOptions { Precision = new Name("Feu") }, _world.OwnerId);

    var exception = Assert.Throws<NotEnoughRemainingTalentPointsException>(() => _character.AddTalent(
      _elementarisme,
      new SetTalentOptions { Precision = new Name("Air") },
      _world.OwnerId));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_character.EntityId, exception.CharacterId);
    Assert.Equal(_character.RemainingTalentPoints, exception.RemainingTalentPoints);
    Assert.Equal(_elementarisme.EntityId, exception.TalentId);
    Assert.Equal(2, exception.Cost);
    Assert.Equal("TalentId", exception.PropertyName);
  }

  [Fact(DisplayName = "AddTalent: it should throw RequiredTalentNotPurchasedException when the required talent was not purchased first.")]
  public void AddTalent_it_should_throw_RequiredTalentNotPurchasedException_when_the_required_talent_was_not_purchased_first()
  {
    var exception = Assert.Throws<RequiredTalentNotPurchasedException>(() => _character.AddTalent(_formationMartiale, _world.OwnerId));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_character.EntityId, exception.CharacterId);
    Assert.Equal(_formationMartiale.EntityId, exception.RequiringTalentId);
    Assert.Equal(_melee.EntityId, exception.RequiredTalentId);
    Assert.Equal("TalentId", exception.PropertyName);
  }

  [Fact(DisplayName = "AddTalent: it should throw TalentCannotBePurchasedMultipleTimesException when the talent is being purchased multiple times.")]
  public void AddTalent_it_should_throw_TalentCannotBePurchasedMultipleTimesException_when_the_talent_is_being_purchased_multiple_times()
  {
    _character.AddTalent(_melee, _world.OwnerId);

    var exception = Assert.Throws<TalentCannotBePurchasedMultipleTimesException>(() => _character.AddTalent(_melee, _world.OwnerId));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_melee.EntityId, exception.TalentId);
    Assert.Equal("TalentId", exception.PropertyName);
  }

  [Fact(DisplayName = "AddTalent: it should throw TalentMaximumCostExceededException when the cost exceeds the maximum cost.")]
  public void AddTalent_it_should_throw_TalentMaximumCostExceededException_when_the_cost_exceeds_the_maximum_cost()
  {
    SetTalentOptions options = new()
    {
      Cost = _melee.MaximumCost + 1
    };
    var exception = Assert.Throws<TalentMaximumCostExceededException>(() => _character.AddTalent(_melee, options, _world.OwnerId));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_melee.EntityId, exception.TalentId);
    Assert.Equal(_melee.Tier, exception.Tier);
    Assert.Equal(_melee.MaximumCost, exception.MaximumCost);
    Assert.Equal(options.Cost, exception.AttemptedCost);
    Assert.Equal("Cost", exception.PropertyName);
  }

  [Fact(DisplayName = "AddTalent: it should throw TalentTierCannotExceedCharacterTierException when the talent tier is greater than the character tier.")]
  public void AddTalent_it_should_throw_TalentTierCannotExceedCharacterTierException_when_the_talent_tier_is_greater_than_the_character_tier()
  {
    var exception = Assert.Throws<TalentTierCannotExceedCharacterTierException>(() => _character.AddTalent(_cuirasse, _world.OwnerId));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_character.EntityId, exception.CharacterId);
    Assert.Equal(_character.Tier, exception.CharacterTier);
    Assert.Equal(_cuirasse.EntityId, exception.TalentId);
    Assert.Equal(_cuirasse.Tier, exception.TalentTier);
    Assert.Equal("TalentId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should account for correct talent points.")]
  public void It_should_account_for_correct_talent_points()
  {
    Assert.Equal(8, _character.AvailableTalentPoints);
    Assert.Equal(0, _character.SpentTalentPoints);
    Assert.Equal(8, _character.RemainingTalentPoints);

    _character.AddTalent(_melee, new SetTalentOptions { Cost = 1 }, _world.OwnerId);
    _character.SetTalent(Guid.NewGuid(), _formationMartiale, _world.OwnerId);
    _character.SetTalent(Guid.NewGuid(), _occultisme, new SetTalentOptions { Cost = 1 }, _world.OwnerId);
    _character.AddTalent(_elementarisme, _world.OwnerId);

    Assert.Equal(8, _character.AvailableTalentPoints);
    Assert.Equal(6, _character.SpentTalentPoints);
    Assert.Equal(2, _character.RemainingTalentPoints);
  }

  [Fact(DisplayName = "RemoveTalent: it should not do anything when the talent was not found.")]
  public void RemoveTalent_it_should_not_do_anything_when_the_talent_was_not_found()
  {
    Assert.Empty(_character.Talents);
    _character.RemoveTalent(Guid.NewGuid(), _world.OwnerId);

    Assert.DoesNotContain(_character.Changes, change => change is Character.TalentRemovedEvent);
  }

  [Fact(DisplayName = "RemoveTalent: it should remove an existing talent.")]
  public void RemoveTalent_it_should_remove_an_existing_talent()
  {
    _character.AddTalent(_melee, _world.OwnerId);
    Guid id = Assert.Single(_character.Talents).Key;

    _character.RemoveTalent(id, _world.OwnerId);
    Assert.Empty(_character.Talents);

    Assert.Contains(_character.Changes, change => change is Character.TalentRemovedEvent e && e.RelationId == id);
  }

  [Fact(DisplayName = "SetTalent: it should add a new talent.")]
  public void SetTalent_it_should_add_a_new_talent_without_options()
  {
    Guid id = Guid.NewGuid();
    _character.SetTalent(id, _melee, _world.OwnerId);

    Assert.Equal(id, Assert.Single(_character.Talents.Keys));

    CharacterTalent talent = Assert.Single(_character.Talents.Values);
    Assert.Equal(_melee.Id, talent.Id);
    Assert.Equal(_melee.Tier + 2, talent.Cost);
    Assert.Null(talent.Precision);
    Assert.Null(talent.Notes);
  }

  [Fact(DisplayName = "SetTalent: it should not do anything when the talent did not change.")]
  public void SetTalent_it_should_not_do_anything_when_the_talent_did_not_change()
  {
    _character.AddTalent(_occultisme, _world.OwnerId);

    Guid id = Guid.NewGuid();
    SetTalentOptions options = new()
    {
      Precision = new Name("Eau")
    };
    _character.SetTalent(id, _elementarisme, options, _world.OwnerId);
    _character.ClearChanges();

    _character.SetTalent(id, _elementarisme, options, _world.OwnerId);
    Assert.Empty(_character.Changes);
    Assert.False(_character.HasChanges);
  }

  [Fact(DisplayName = "SetTalent: it should throw ArgumentException when the talent resides in another world.")]
  public void SetTalent_it_should_throw_ArgumentException_when_the_talent_resides_in_another_world()
  {
    var exception = Assert.Throws<ArgumentException>(() => _character.SetTalent(Guid.NewGuid(), _otherWorldTalent, _world.OwnerId));
    Assert.StartsWith("The talent does not reside in the same world as the character.", exception.Message);
    Assert.Equal("talent", exception.ParamName);
  }

  [Fact(DisplayName = "SetTalent: it should throw ArgumentOutOfRangeException when the cost is negative.")]
  public void SetTalent_it_should_throw_ArgumentOutOfRangeException_when_the_cost_is_negative()
  {
    SetTalentOptions options = new()
    {
      Cost = -_melee.MaximumCost
    };
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _character.SetTalent(Guid.NewGuid(), _melee, options, _world.OwnerId));
    Assert.StartsWith("The talent cost cannot be negative.", exception.Message);
    Assert.Equal("options", exception.ParamName);
  }

  [Fact(DisplayName = "SetTalent: it should throw RequiredTalentNotPurchasedException when the required talent was not purchased first.")]
  public void SetTalent_it_should_throw_RequiredTalentNotPurchasedException_when_the_required_talent_was_not_purchased_first()
  {
    var exception = Assert.Throws<RequiredTalentNotPurchasedException>(() => _character.SetTalent(Guid.NewGuid(), _formationMartiale, _world.OwnerId));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_character.EntityId, exception.CharacterId);
    Assert.Equal(_formationMartiale.EntityId, exception.RequiringTalentId);
    Assert.Equal(_melee.EntityId, exception.RequiredTalentId);
    Assert.Equal("TalentId", exception.PropertyName);
  }

  [Fact(DisplayName = "SetTalent: it should throw NotEnoughRemainingTalentPointsException when there is not enough remaining talent points.")]
  public void SetTalent_it_should_throw_NotEnoughRemainingTalentPointsException_when_there_is_not_enough_remaining_talent_points()
  {
    _character.AddTalent(_melee, _world.OwnerId);
    _character.AddTalent(_formationMartiale, _world.OwnerId);
    _character.AddTalent(_occultisme, new SetTalentOptions
    {
      Cost = 1,
      Notes = new Description("Discounted by Aspect: Tenace")
    }, _world.OwnerId);
    _character.AddTalent(_elementarisme, new SetTalentOptions { Precision = new Name("Feu") }, _world.OwnerId);

    var exception = Assert.Throws<NotEnoughRemainingTalentPointsException>(() => _character.SetTalent(
      Guid.NewGuid(),
      _elementarisme,
      new SetTalentOptions { Precision = new Name("Air") },
      _world.OwnerId));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_character.EntityId, exception.CharacterId);
    Assert.Equal(_character.RemainingTalentPoints, exception.RemainingTalentPoints);
    Assert.Equal(_elementarisme.EntityId, exception.TalentId);
    Assert.Equal(2, exception.Cost);
    Assert.Equal("TalentId", exception.PropertyName);
  }

  [Fact(DisplayName = "SetTalent: it should throw TalentCannotBePurchasedMultipleTimesException when the talent is being purchased multiple times.")]
  public void SetTalent_it_should_throw_TalentCannotBePurchasedMultipleTimesException_when_the_talent_is_being_purchased_multiple_times()
  {
    _character.AddTalent(_melee, _world.OwnerId);

    var exception = Assert.Throws<TalentCannotBePurchasedMultipleTimesException>(() => _character.SetTalent(Guid.NewGuid(), _melee, _world.OwnerId));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_melee.EntityId, exception.TalentId);
    Assert.Equal("TalentId", exception.PropertyName);
  }

  [Fact(DisplayName = "SetTalent: it should throw TalentMaximumCostExceededException when the cost exceeds the maximum cost.")]
  public void SetTalent_it_should_throw_TalentMaximumCostExceededException_when_the_cost_exceeds_the_maximum_cost()
  {
    _character.AddTalent(_melee, _world.OwnerId);
    Guid id = Assert.Single(_character.Talents.Keys);

    SetTalentOptions options = new()
    {
      Cost = _melee.MaximumCost + 1
    };
    var exception = Assert.Throws<TalentMaximumCostExceededException>(() => _character.SetTalent(id, _melee, options, _world.OwnerId));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_melee.EntityId, exception.TalentId);
    Assert.Equal(_melee.Tier, exception.Tier);
    Assert.Equal(_melee.MaximumCost, exception.MaximumCost);
    Assert.Equal(options.Cost, exception.AttemptedCost);
    Assert.Equal("Cost", exception.PropertyName);
  }

  [Fact(DisplayName = "SetTalent: it should throw TalentTierCannotExceedCharacterTierException when the talent tier is greater than the character tier.")]
  public void SetTalent_it_should_throw_TalentTierCannotExceedCharacterTierException_when_the_talent_tier_is_greater_than_the_character_tier()
  {
    var exception = Assert.Throws<TalentTierCannotExceedCharacterTierException>(() => _character.SetTalent(Guid.NewGuid(), _cuirasse, _world.OwnerId));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(_character.EntityId, exception.CharacterId);
    Assert.Equal(_character.Tier, exception.CharacterTier);
    Assert.Equal(_cuirasse.EntityId, exception.TalentId);
    Assert.Equal(_cuirasse.Tier, exception.TalentTier);
    Assert.Equal("TalentId", exception.PropertyName);
  }

  [Fact(DisplayName = "SetTalent: it should update an existing talent.")]
  public void SetTalent_it_should_update_an_existing_talent()
  {
    _character.AddTalent(_melee, _world.OwnerId);
    _character.AddTalent(_occultisme, _world.OwnerId);
    _character.AddTalent(_elementarisme, new SetTalentOptions { Precision = new Name("Terre") }, _world.OwnerId);

    Guid id = Guid.NewGuid();
    _character.SetTalent(id, _elementarisme, _world.OwnerId);

    SetTalentOptions options = new()
    {
      Precision = new Name("Esprit")
    };
    _character.SetTalent(id, _elementarisme, options, _world.OwnerId);

    CharacterTalent talent = _character.Talents[id];
    Assert.Equal(_elementarisme.Id, talent.Id);
    Assert.Equal(_elementarisme.Tier + 2, talent.Cost);
    Assert.Equal(options.Precision, talent.Precision);
    Assert.Equal(options.Notes, talent.Notes);
  }
}
