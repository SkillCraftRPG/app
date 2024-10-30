using FluentValidation;
using SkillCraft.Contracts.Items;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Domain.Items.Properties;

public record WeaponProperties : PropertiesBase
{
  [JsonIgnore]
  public override ItemCategory Category { get; } = ItemCategory.Weapon;

  public int Attack { get; }
  public int? Resistance { get; }

  [JsonIgnore]
  private readonly HashSet<WeaponTrait> _traits;
  public IReadOnlyCollection<WeaponTrait> Traits => _traits.ToArray().AsReadOnly();

  public IReadOnlyCollection<WeaponDamage> Damages { get; }
  public IReadOnlyCollection<WeaponDamage> VersatileDamages { get; }

  public WeaponRange? Range { get; }
  public int? ReloadCount { get; }

  [JsonIgnore]
  public override int Size => 4 /* Attack */ + 4 /* Resistance */ + _traits.Count * 4
    + Damages.Sum(d => d.Size) + VersatileDamages.Sum(d => d.Size)
    + WeaponRange.Size + 4 /* ReloadCount */;

  [JsonConstructor]
  public WeaponProperties(
    int attack,
    int? resistance,
    IReadOnlyCollection<WeaponTrait> traits,
    IReadOnlyCollection<WeaponDamage> damages,
    IReadOnlyCollection<WeaponDamage> versatileDamages,
    WeaponRange? range,
    int? reloadCount)
  {
    Attack = attack;
    Resistance = resistance;
    _traits = [.. traits];

    Damages = damages.ToArray().AsReadOnly();
    VersatileDamages = versatileDamages.ToArray().AsReadOnly();

    Range = range;
    ReloadCount = reloadCount;

    new Validator().ValidateAndThrow(this);
  }

  public bool HasTrait(WeaponTrait trait) => _traits.Contains(trait);

  private class Validator : AbstractValidator<WeaponProperties>
  {
    public Validator()
    {
      When(x => x.Resistance != null, () => RuleFor(x => x.Resistance).GreaterThan(0));
      RuleForEach(x => x.Traits).IsInEnum();

      When(x => x.VersatileDamages.Count > 0, () => RuleFor(x => x).Must(x => x.HasTrait(WeaponTrait.Versatile))
        .WithErrorCode("WeaponPropertiesValidator")
        .WithMessage($"'{nameof(Traits)}' must include '{WeaponTrait.Versatile}' when versatile damages are specified."));

      When(x => x.Range != null, () => RuleFor(x => x).Must(x => x.HasTrait(WeaponTrait.Range))
        .WithErrorCode("WeaponPropertiesValidator")
        .WithMessage($"'{nameof(Traits)}' must include '{WeaponTrait.Range}' when a range is specified."));
      When(x => x.ReloadCount != null, () =>
      {
        RuleFor(x => x.ReloadCount).GreaterThan(1);
        RuleFor(x => x).Must(x => x.HasTrait(WeaponTrait.Reload))
          .WithErrorCode("WeaponPropertiesValidator")
          .WithMessage($"'{nameof(Traits)}' must include {WeaponTrait.Reload} when a reload count is specified.");
      });
    }
  }
}
