using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Worlds;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Domain.Personalities;

[Trait(Traits.Category, Categories.Unit)]
public class PersonalityTests
{
  private readonly Personality _personality = new(WorldId.NewId(), new Name("Timide"), UserId.NewId());

  [Fact(DisplayName = "It should throw ArgumentException when setting a customization that is not a gift.")]
  public void It_should_throw_ArgumentException_when_setting_a_customization_that_is_not_a_gift()
  {
    Customization gift = new(_personality.WorldId, CustomizationType.Disability, new Name("Cleptomane"), new UserId(_personality.CreatedBy));
    var exception = Assert.Throws<ArgumentException>(() => _personality.SetGift(gift));
    Assert.StartsWith("The customization is not a gift.", exception.Message);
    Assert.Equal("gift", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when setting a gift from another world.")]
  public void It_should_throw_ArgumentException_when_setting_a_gift_from_another_world()
  {
    Customization gift = new(WorldId.NewId(), CustomizationType.Gift, new Name("Féroce"), UserId.NewId());
    var exception = Assert.Throws<ArgumentException>(() => _personality.SetGift(gift));
    Assert.StartsWith("The gift does not reside in the same world as the personality.", exception.Message);
    Assert.Equal("gift", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when the attribute is not defined.")]
  public void It_should_throw_ArgumentOutOfRangeException_when_the_skill_is_not_defined()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _personality.Attribute = (Attribute)(-1));
    Assert.Equal("Attribute", exception.ParamName);
  }
}
