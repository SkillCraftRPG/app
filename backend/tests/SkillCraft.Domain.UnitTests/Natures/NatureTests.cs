using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Worlds;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Domain.Natures;

[Trait(Traits.Category, Categories.Unit)]
public class NatureTests
{
  private readonly UserId _userId = UserId.NewId();
  private readonly Nature _nature;
  public NatureTests()
  {
    _nature = new(WorldId.NewId(), new Name("Timide"), _userId);
  }

  [Fact(DisplayName = "It should throw ArgumentException when setting a gift from another world.")]
  public void It_should_throw_ArgumentException_when_setting_a_gift_from_another_world()
  {
    Customization gift = new(WorldId.NewId(), CustomizationType.Gift, new Name("Féroce"), UserId.NewId());
    var exception = Assert.Throws<ArgumentException>(() => _nature.SetGift(gift));
    Assert.StartsWith("The gift does not reside in the same world as the nature.", exception.Message);
    Assert.Equal("gift", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when the attribute is not defined.")]
  public void It_should_throw_ArgumentOutOfRangeException_when_the_skill_is_not_defined()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _nature.Attribute = (Attribute)(-1));
    Assert.Equal("Attribute", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw CustomizationIsNotGiftException when setting a customization that is not a Gift.")]
  public void It_should_throw_CustomizationIsNotGiftException_when_setting_a_customization_that_is_not_a_Gift()
  {
    Customization disability = new(_nature.WorldId, CustomizationType.Disability, new Name("Chaotique"), _userId);
    var exception = Assert.Throws<CustomizationIsNotGiftException>(() => _nature.SetGift(disability));
    Assert.Equal(disability.WorldId.ToGuid(), exception.WorldId);
    Assert.Equal(disability.EntityId, exception.CustomizationId);
    Assert.Equal(disability.Type, exception.CustomizationType);
    Assert.Equal("GiftId", exception.PropertyName);
  }
}
