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

  public WeaponRange? AmmunitionRange { get; }
  public WeaponRange? ThrownRange { get; }
  public int? ReloadCount { get; }

  [JsonIgnore]
  public override int Size => 4 /* Attack */ + 4 /* Resistance */ + _traits.Count * 4
    + Damages.Sum(d => d.Size) + VersatileDamages.Sum(d => d.Size)
    + (WeaponRange.Size * 2) + 4 /* ReloadCount */;

  [JsonConstructor]
  public WeaponProperties(
    int attack,
    int? resistance,
    IReadOnlyCollection<WeaponTrait> traits,
    IReadOnlyCollection<WeaponDamage> damages,
    IReadOnlyCollection<WeaponDamage> versatileDamages,
    WeaponRange? ammunitionRange,
    WeaponRange? thrownRange,
    int? reloadCount)
  {
    Attack = attack;
    Resistance = resistance;
    _traits = [.. traits];

    Damages = damages.ToArray().AsReadOnly();
    VersatileDamages = versatileDamages.ToArray().AsReadOnly();

    AmmunitionRange = ammunitionRange;
    ThrownRange = thrownRange;
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

      When(x => x.ReloadCount != null, () => RuleFor(x => x.ReloadCount).GreaterThan(1));
    }
  }
}
