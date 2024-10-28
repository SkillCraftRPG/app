using FluentValidation;
using SkillCraft.Contracts.Items;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Items.Validators;

internal class UpdateItemValidator : AbstractValidator<UpdateItemPayload>
{
  public UpdateItemValidator(ItemCategory category)
  {
    When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    When(x => x.Value?.Value != null, () => RuleFor(x => x.Value!.Value).GreaterThanOrEqualTo(0));
    When(x => x.Weight?.Value != null, () => RuleFor(x => x.Weight!.Value).GreaterThanOrEqualTo(0));

    ApplyPropertiesRules(category);
  }

  private void ApplyPropertiesRules(ItemCategory category)
  {
    if (category == ItemCategory.Consumable)
    {
      When(x => x.Consumable != null, () => RuleFor(x => x.Consumable!).SetValidator(new ConsumablePropertiesValidator()));
    }
    else
    {
      RuleFor(x => x.Consumable).Null();
    }

    if (category == ItemCategory.Container)
    {
      When(x => x.Container != null, () => RuleFor(x => x.Container!).SetValidator(new ContainerPropertiesValidator()));
    }
    else
    {
      RuleFor(x => x.Container).Null();
    }

    if (category == ItemCategory.Device)
    {
      When(x => x.Device != null, () => RuleFor(x => x.Device!).SetValidator(new DevicePropertiesValidator()));
    }
    else
    {
      RuleFor(x => x.Device).Null();
    }

    if (category == ItemCategory.Equipment)
    {
      When(x => x.Equipment != null, () => RuleFor(x => x.Equipment!).SetValidator(new EquipmentPropertiesValidator()));
    }
    else
    {
      RuleFor(x => x.Equipment).Null();
    }

    if (category == ItemCategory.Miscellaneous)
    {
      When(x => x.Miscellaneous != null, () => RuleFor(x => x.Miscellaneous!).SetValidator(new MiscellaneousPropertiesValidator()));
    }
    else
    {
      RuleFor(x => x.Miscellaneous).Null();
    }

    if (category == ItemCategory.Money)
    {
      When(x => x.Money != null, () => RuleFor(x => x.Money!).SetValidator(new MoneyPropertiesValidator()));
    }
    else
    {
      RuleFor(x => x.Money).Null();
    }

    if (category == ItemCategory.Weapon)
    {
      When(x => x.Weapon != null, () => RuleFor(x => x.Weapon!).SetValidator(new WeaponPropertiesValidator()));
    }
    else
    {
      RuleFor(x => x.Weapon).Null();
    }
  }
}
