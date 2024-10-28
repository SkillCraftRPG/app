using FluentValidation;
using SkillCraft.Contracts.Items;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Domain.Items.Properties;

public record ConsumableProperties : PropertiesBase, IConsumableProperties
{
  public override ItemCategory Category { get; } = ItemCategory.Consumable;

  public int? Charges { get; }
  public bool RemoveWhenEmpty { get; }
  public Guid? ReplaceWithItemWhenEmptyId { get; }

  public ConsumableProperties(IConsumableProperties consumable) : this(consumable.Charges, consumable.RemoveWhenEmpty, consumable.ReplaceWithItemWhenEmptyId)
  {
  }

  public ConsumableProperties(int? charges, bool removeWhenEmpty, Guid? replaceWithItemWhenEmptyId)
  {
    Charges = charges;
    RemoveWhenEmpty = removeWhenEmpty;
    ReplaceWithItemWhenEmptyId = replaceWithItemWhenEmptyId;
    new ConsumablePropertiesValidator().ValidateAndThrow(this);
  }
}
