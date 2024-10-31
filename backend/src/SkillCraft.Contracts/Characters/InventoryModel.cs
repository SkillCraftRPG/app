using SkillCraft.Contracts.Items;

namespace SkillCraft.Contracts.Characters;

public class InventoryModel
{
  public Guid Id { get; set; }

  public ItemModel Item { get; set; }

  public Guid? ContainingItemId { get; set; }
  public int Quantity { get; set; }
  public bool? IsAttuned { get; set; }
  public bool IsEquipped { get; set; }
  public bool IsIdentified { get; set; }
  public bool? IsProficient { get; set; }
  public Skill? Skill { get; set; }
  public int? RemainingCharges { get; set; }
  public int? RemainingResistance { get; set; }
  public string? NameOverride { get; set; }
  public string? DescriptionOverride { get; set; }
  public double? ValueOverride { get; set; }

  public InventoryModel() : this(new ItemModel())
  {
  }

  public InventoryModel(ItemModel item)
  {
    Item = item;
  }

  public override bool Equals(object? obj) => obj is InventoryModel other && other.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{Item.Name} | {GetType()} (Id={Id})";
}
