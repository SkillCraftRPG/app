using FluentValidation;
using SkillCraft.Contracts.Items;

namespace SkillCraft.Domain.Items.Properties;

public record ConsumableProperties : PropertiesBase
{
  [JsonIgnore]
  public override ItemCategory Category { get; } = ItemCategory.Consumable;

  public int? Charges { get; }
  public bool RemoveWhenEmpty { get; }
  public ItemId? ReplaceWithItemWhenEmptyId { get; }

  [JsonIgnore]
  public override int Size { get; } = 4 /* Charges */ + 1 /* RemoveWhenEmpty */ + 4 /* ReplaceWithItemWhenEmptyId */;

  [JsonConstructor]
  public ConsumableProperties(int? charges, bool removeWhenEmpty, ItemId? replaceWithItemWhenEmptyId)
  {
    Charges = charges;
    RemoveWhenEmpty = removeWhenEmpty;
    ReplaceWithItemWhenEmptyId = replaceWithItemWhenEmptyId;
    new Validator().ValidateAndThrow(this);
  }

  private class Validator : AbstractValidator<ConsumableProperties>
  {
    public Validator()
    {
      When(x => x.Charges != null, () => RuleFor(x => x.Charges).GreaterThan(0));
      When(x => x.RemoveWhenEmpty, () => RuleFor(x => x.ReplaceWithItemWhenEmptyId).Null());
      When(x => x.ReplaceWithItemWhenEmptyId != null, () =>
      {
        RuleFor(x => x.RemoveWhenEmpty).NotEqual(true);
        RuleFor(x => x.ReplaceWithItemWhenEmptyId).NotEmpty();
      });
    }
  }
}
