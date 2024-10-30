using FluentValidation;

namespace SkillCraft.Domain.Items.Properties;

public record WeaponRange
{
  public const int Size = 4 /* Normal */ + 4 /* Long */;

  public int? Normal { get; }
  public int? Long { get; }

  public WeaponRange(int? normal, int? @long)
  {
    Normal = normal;
    Long = @long;
  }

  private class Validator : AbstractValidator<WeaponRange>
  {
    public Validator()
    {
      When(x => x.Normal != null, () => RuleFor(x => x.Normal).GreaterThan(0));
      When(x => x.Long != null, () => RuleFor(x => x.Long).GreaterThan(0));
      When(x => x.Normal != null && x.Long != null, () =>
      {
        RuleFor(x => x.Normal).LessThan(x => x.Long);
        RuleFor(x => x.Long).GreaterThan(x => x.Normal);
      });
    }
  }
}
