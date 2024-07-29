using FluentValidation.Results;

namespace SkillCraft.Domain;

[Trait(Traits.Category, Categories.Unit)]
public class LocaleUnitTests
{
  [Theory(DisplayName = "ctor: it should construct the correct locale.")]
  [InlineData("fr")]
  [InlineData("  FR-CA  ")]
  public void Ctor_it_should_construct_the_correct_locale(string code)
  {
    CultureInfo culture = CultureInfo.GetCultureInfo(code.Trim());

    LocaleUnit locale = new(code);
    Assert.Equal(culture.Name, locale.Code);
    Assert.Same(culture, locale.Culture);
  }

  [Theory(DisplayName = "ctor: it should throw ValidationException when the code is not valid.")]
  [InlineData("    ")]
  [InlineData("fr-CACACACACACACA")]
  [InlineData(" test ")]
  public void Ctor_it_should_throw_ValidationException_when_the_code_is_not_valid(string code)
  {
    var exception = Assert.Throws<FluentValidation.ValidationException>(() => new LocaleUnit(code));

    if (string.IsNullOrWhiteSpace(code))
    {
      Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Code" && (string)e.AttemptedValue == string.Empty);
    }
    else if (code.Length > 16)
    {
      Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Code" && (string)e.AttemptedValue == code);
    }
    else
    {
      ValidationFailure error = Assert.Single(exception.Errors);
      Assert.Equal("LocaleValidator", error.ErrorCode);
      Assert.Equal("'Code' may not be the invariant culture, nor a user-defined culture.", error.ErrorMessage);
      Assert.Equal("Code", error.PropertyName);
      Assert.Equal(code.Trim(), error.AttemptedValue);
    }
  }
}
