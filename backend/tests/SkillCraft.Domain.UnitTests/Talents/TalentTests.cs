using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Talents;

[Trait(Traits.Category, Categories.Unit)]
public class TalentTests
{
  private readonly Talent _talent = new(WorldId.NewId(), tier: 1, new Name("Cuirassé"), UserId.NewId());

  [Theory(DisplayName = "It should return the correct maximum cost.")]
  [InlineData(0, 2)]
  [InlineData(1, 3)]
  [InlineData(2, 4)]
  [InlineData(3, 5)]
  public void It_should_return_the_correct_maximum_cost(int tier, int maximumCost)
  {
    Talent talent = new(WorldId.NewId(), tier, _talent.Name, UserId.NewId());
    Assert.Equal(maximumCost, talent.MaximumCost);
  }

  [Fact(DisplayName = "It should throw ArgumentException when requiring a talent from another world.")]
  public void It_should_throw_ArgumentException_when_setting_a_gift_from_another_world()
  {
    Talent talent = new(WorldId.NewId(), tier: 0, new Name("Formation martiale"), UserId.NewId());
    var exception = Assert.Throws<ArgumentException>(() => _talent.SetRequiredTalent(talent));
    Assert.StartsWith("The required talent does not reside in the same world as the requiring talent.", exception.Message);
    Assert.Equal("requiredTalent", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when requiring a talent of a higher tier.")]
  public void It_should_throw_ArgumentException_when_setting_a_customization_that_is_not_a_gift()
  {
    Talent talent = new(_talent.WorldId, tier: 2, new Name("Protection"), new UserId(_talent.CreatedBy));
    var exception = Assert.Throws<ArgumentException>(() => _talent.SetRequiredTalent(talent));
    Assert.StartsWith("The required talent tier must be inferior or equal to the requiring talent tier.", exception.Message);
    Assert.Equal("requiredTalent", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when the tier is not valid.")]
  public void It_should_throw_ArgumentOutOfRangeException_when_the_tier_is_not_valid()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Talent(_talent.WorldId, tier: 10, _talent.Name, new UserId(_talent.CreatedBy)));
    Assert.Equal("tier", exception.ParamName);
  }

  [Fact(DisplayName = "ThrowIfMaximumCostExceeded: it should not do anything when the talent maximum cost was not exceeded.")]
  public void ThrowIfMaximumCostExceeded_it_should_not_do_anything_when_the_talent_maximum_cost_was_not_exceeded()
  {
    _talent.ThrowIfMaximumCostExceeded(_talent.MaximumCost, propertyName: string.Empty);
  }

  [Fact(DisplayName = "ThrowIfMaximumCostExceeded: it should throw TalentMaximumCostExceededException when the maximum cost was exceeded.")]
  public void ThrowIfMaximumCostExceeded_it_should_throw_TalentMaximumCostExceededException_when_the_maximum_cost_was_exceeded()
  {
    string propertyName = "cost";
    int cost = _talent.MaximumCost + 1;
    var exception = Assert.Throws<TalentMaximumCostExceededException>(() => _talent.ThrowIfMaximumCostExceeded(cost, propertyName));
    Assert.Equal(_talent.WorldId.ToGuid(), exception.WorldId);
    Assert.Equal(_talent.EntityId, exception.TalentId);
    Assert.Equal(_talent.Tier, exception.Tier);
    Assert.Equal(_talent.MaximumCost, exception.MaximumCost);
    Assert.Equal(cost, exception.AttemptedCost);
    Assert.Equal(propertyName, exception.PropertyName);
  }
}
