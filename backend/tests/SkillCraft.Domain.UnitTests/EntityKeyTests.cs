using FluentValidation;
using SkillCraft.Contracts;

namespace SkillCraft.Domain;

[Trait(Traits.Category, Categories.Unit)]
public class EntityKeyTests
{
  [Fact(DisplayName = "It should create the correct entity key.")]
  public void It_should_create_the_correct_entity_key()
  {
    EntityType type = EntityType.Character;
    Guid id = Guid.NewGuid();
    EntityKey entity = new(type, id);
    Assert.Equal(type, entity.Type);
    Assert.Equal(id, entity.Id);
  }

  [Fact(DisplayName = "It should throw ValidationException when the arguments are not valid.")]
  public void It_should_throw_ValidationException_when_the_arguments_are_not_valid()
  {
    var exception = Assert.Throws<ValidationException>(() => new EntityKey((EntityType)(-1), Guid.Empty));
    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "EnumValidator" && e.PropertyName == "Type");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Id");
  }
}
