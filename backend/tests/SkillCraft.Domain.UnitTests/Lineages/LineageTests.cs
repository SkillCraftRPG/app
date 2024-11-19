using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Lineages;

[Trait(Traits.Category, Categories.Unit)]
public class LineageTests
{
  private readonly Lineage _lineage = new(WorldId.NewId(), parent: null, new Name("Nain"), UserId.NewId());

  [Fact(DisplayName = "It should throw ArgumentException when setting a parent from another world.")]
  public void It_should_throw_ArgumentException_when_setting_a_parent_from_another_world()
  {
    Lineage parent = new(WorldId.NewId(), parent: null, new Name("Nains"), UserId.NewId());
    var exception = Assert.Throws<ArgumentException>(() => new Lineage(_lineage.WorldId, parent, _lineage.Name, new UserId(_lineage.CreatedBy)));
    Assert.StartsWith("The parent lineage does not reside in the same world as the lineage.", exception.Message);
    Assert.Equal("parent", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentException when setting a parent with a parent.")]
  public void It_should_throw_ArgumentException_when_setting_a_parent_with_a_parent()
  {
    Lineage root = new(_lineage.WorldId, parent: null, new Name("Humanoïdes"), new UserId(_lineage.CreatedBy));
    Lineage parent = new(_lineage.WorldId, root, new Name("Nains"), new UserId(_lineage.CreatedBy));
    var exception = Assert.Throws<ArgumentException>(() => new Lineage(_lineage.WorldId, parent, _lineage.Name, new UserId(_lineage.CreatedBy)));
    Assert.StartsWith("The parent lineage cannot have a parent lineage.", exception.Message);
    Assert.Equal("parent", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when removing an empty trait ID.")]
  public void It_should_throw_ArgumentOutOfRangeException_when_removing_an_empty_trait_Id()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _lineage.RemoveTrait(Guid.Empty));
    Assert.Equal("id", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when setting an empty trait ID.")]
  public void It_should_throw_ArgumentOutOfRangeException_when_setting_an_empty_trait_Id()
  {
    Trait trait = new(new Name("Vagabond"), Description: null);
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _lineage.SetTrait(Guid.Empty, trait));
    Assert.Equal("id", exception.ParamName);
  }
}
