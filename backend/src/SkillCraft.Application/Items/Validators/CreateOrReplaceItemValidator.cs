using FluentValidation;
using SkillCraft.Contracts.Items;
using SkillCraft.Domain.Items.Properties;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Application.Items.Validators;

internal class CreateOrReplaceItemValidator : AbstractValidator<CreateOrReplaceItemPayload>
{
  public CreateOrReplaceItemValidator(ItemCategory? category = null)
  {
    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    When(x => x.Value != null, () => RuleFor(x => x.Value).GreaterThanOrEqualTo(0));
    When(x => x.Weight != null, () => RuleFor(x => x.Weight).GreaterThanOrEqualTo(0));

    ApplyPropertiesRules(category);
  }

  private void ApplyPropertiesRules(ItemCategory? category)
  {
    if (category == null || category == ItemCategory.Consumable)
    {
      When(x => x.Consumable != null, () => RuleFor(x => x.Consumable!).SetValidator(new ConsumablePropertiesValidator()))
        .Otherwise(() => RuleFor(x => x.Consumable).NotNull());
    }
    else
    {
      RuleFor(x => x.Consumable).Null();
    }

    if (category == null || category == ItemCategory.Container)
    {
      When(x => x.Container != null, () => RuleFor(x => x.Container!).SetValidator(new ContainerPropertiesValidator()))
        .Otherwise(() => RuleFor(x => x.Container).NotNull());
    }
    else
    {
      RuleFor(x => x.Container).Null();
    }

    if (category == null || category == ItemCategory.Device)
    {
      When(x => x.Device != null, () => RuleFor(x => x.Device!).SetValidator(new DevicePropertiesValidator()))
        .Otherwise(() => RuleFor(x => x.Device).NotNull());
    }
    else
    {
      RuleFor(x => x.Device).Null();
    }

    if (category == null || category == ItemCategory.Equipment)
    {
      When(x => x.Equipment != null, () => RuleFor(x => x.Equipment!).SetValidator(new EquipmentPropertiesValidator()))
        .Otherwise(() => RuleFor(x => x.Equipment).NotNull());
    }
    else
    {
      RuleFor(x => x.Equipment).Null();
    }

    if (category == null || category == ItemCategory.Miscellaneous)
    {
      When(x => x.Miscellaneous != null, () => RuleFor(x => x.Miscellaneous!).SetValidator(new MiscellaneousPropertiesValidator()))
        .Otherwise(() => RuleFor(x => x.Miscellaneous).NotNull());
    }
    else
    {
      RuleFor(x => x.Miscellaneous).Null();
    }

    if (category == null || category == ItemCategory.Money)
    {
      When(x => x.Money != null, () => RuleFor(x => x.Money!).SetValidator(new MoneyPropertiesValidator()))
        .Otherwise(() => RuleFor(x => x.Money).NotNull());
    }
    else
    {
      RuleFor(x => x.Money).Null();
    }

    if (category == null || category == ItemCategory.Weapon)
    {
      When(x => x.Weapon != null, () => RuleFor(x => x.Weapon!).SetValidator(new WeaponPropertiesValidator()))
        .Otherwise(() => RuleFor(x => x.Weapon).NotNull());
    }
    else
    {
      RuleFor(x => x.Weapon).Null();
    }

    if (category == null)
    {
      RuleFor(x => x).Must(HaveExactlyOnePropertiesSet)
        .WithErrorCode(nameof(CreateOrReplaceItemValidator))
        .WithMessage(p => $"Exactly one of the following must be provided: {nameof(p.Consumable)}, {nameof(p.Container)}, {nameof(p.Device)}, {nameof(p.Equipment)}, {nameof(p.Miscellaneous)}, {nameof(p.Money)}, {nameof(p.Weapon)}.");
    }
  }

  private static bool HaveExactlyOnePropertiesSet(CreateOrReplaceItemPayload payload)
  {
    int count = 0;
    if (payload.Consumable != null)
    {
      count++;
    }
    if (payload.Container != null)
    {
      count++;
    }
    if (payload.Device != null)
    {
      count++;
    }
    if (payload.Equipment != null)
    {
      count++;
    }
    if (payload.Miscellaneous != null)
    {
      count++;
    }
    if (payload.Money != null)
    {
      count++;
    }
    if (payload.Weapon != null)
    {
      count++;
    }
    return count == 1;
  }
}
