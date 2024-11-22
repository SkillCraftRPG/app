using FluentValidation;
using FluentValidation.Results;
using SkillCraft.Contracts.Characters;

namespace SkillCraft.Domain.Characters;

[Trait(Traits.Category, Categories.Unit)]
public class BonusTests
{
  [Fact(DisplayName = "It should construct a valid bonus.")]
  public void It_should_construct_a_valid_bonus()
  {
    BonusCategory category = BonusCategory.Miscellaneous;
    string target = MiscellaneousBonusTarget.Stamina.ToString();
    int value = 5;
    bool isTemporary = false;
    Name precision = new("Talent : Discipline");
    Description notes = new("**Discipline.** Confère au personnage un bonus permanent de 5 points d’Énergie.");

    Bonus bonus = new(category, target, value, isTemporary, precision, notes);

    Assert.Equal(category, bonus.Category);
    Assert.Equal(target, bonus.Target);
    Assert.Equal(value, bonus.Value);
    Assert.Equal(isTemporary, bonus.IsTemporary);
    Assert.Same(precision, bonus.Precision);
    Assert.Same(notes, bonus.Notes);
  }

  [Fact(DisplayName = "It should throw ValidationException when constructing an invalid bonus.")]
  public void It_should_throw_ValidationException_when_constructing_an_invalid_bonus()
  {
    var exception = Assert.Throws<ValidationException>(() => new Bonus(BonusCategory.Attribute, target: "Test", value: 0));
    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "BonusTargetValidator" && e.PropertyName == "Target");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEqualValidator" && e.PropertyName == "Value");

    exception = Assert.Throws<ValidationException>(() => new Bonus((BonusCategory)(-1), target: "Test", value: -1));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("EnumValidator", error.ErrorCode);
    Assert.Equal("Category", error.PropertyName);
  }
}
