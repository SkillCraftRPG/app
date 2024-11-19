using SkillCraft.Contracts;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Castes;

[Trait(Traits.Category, Categories.Unit)]
public class CasteTests
{
  private readonly Caste _caste = new(WorldId.NewId(), new Name("Amuseur"), UserId.NewId());

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when removing an empty feature ID.")]
  public void It_should_throw_ArgumentOutOfRangeException_when_removing_an_empty_feature_Id()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _caste.RemoveFeature(Guid.Empty));
    Assert.Equal("id", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when setting an empty feature ID.")]
  public void It_should_throw_ArgumentOutOfRangeException_when_setting_an_empty_feature_Id()
  {
    Feature feature = new(new Name("Vagabond"), Description: null);
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _caste.SetFeature(Guid.Empty, feature));
    Assert.Equal("id", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when the skill is not defined.")]
  public void It_should_throw_ArgumentOutOfRangeException_when_the_skill_is_not_defined()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _caste.Skill = (Skill)(-1));
    Assert.Equal("Skill", exception.ParamName);
  }
}
