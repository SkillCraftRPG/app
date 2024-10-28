using FluentValidation;
using SkillCraft.Contracts;

namespace SkillCraft.Domain.Items.Properties;

public record WeaponDamage
{
  public Roll Roll { get; }
  public DamageType Type { get; }

  public WeaponDamage(Roll roll, DamageType type)
  {
    Roll = roll;
    Type = type;
    new Validator().ValidateAndThrow(this);
  }

  private class Validator : AbstractValidator<WeaponDamage>
  {
    public Validator()
    {
      RuleFor(x => x.Type).IsInEnum();
    }
  }
}
