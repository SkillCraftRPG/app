using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Customizations;

[Trait(Traits.Category, Categories.Unit)]
public class CustomizationTests
{
  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when the type is not defined.")]
  public void It_should_throw_ArgumentOutOfRangeException_when_the_type_is_not_defined()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Customization(
      WorldId.NewId(),
      (CustomizationType)(-1),
      new Name("Médecine de brousse"),
      UserId.NewId()));
    Assert.Equal("type", exception.ParamName);
  }
}
