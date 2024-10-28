using FluentValidation;
using SkillCraft.Contracts.Items;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Domain.Items.Properties;

public record EquipmentProperties : PropertiesBase
{
  public override ItemCategory Category { get; } = ItemCategory.Equipment;

  public int Defense { get; }
  public int? Resistance { get; }

  [JsonIgnore]
  private readonly HashSet<EquipmentTrait> _traits;
  public IReadOnlyCollection<EquipmentTrait> Traits => _traits.ToArray().AsReadOnly();

  public EquipmentProperties(IEquipmentProperties equipment) : this(equipment.Defense, equipment.Resistance, equipment.Traits)
  {
  }

  [JsonConstructor]
  public EquipmentProperties(int defense, int? resistance, IReadOnlyCollection<EquipmentTrait> traits)
  {
    Defense = defense;
    Resistance = resistance;
    _traits = [.. traits];

    new Validator().ValidateAndThrow(this);
  }

  public bool HasTrait(EquipmentTrait trait) => _traits.Contains(trait);

  private class Validator : AbstractValidator<EquipmentProperties>
  {
    public Validator()
    {
      RuleFor(x => x.Defense).GreaterThanOrEqualTo(0);
      When(x => x.Resistance != null, () => RuleFor(x => x.Resistance).GreaterThan(0));
      RuleForEach(x => x.Traits).IsInEnum();
    }
  }
}
