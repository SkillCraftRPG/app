namespace SkillCraft.Contracts.Items.Properties;

public interface IEquipmentProperties
{
  int Defense { get; }
  int? Resistance { get; }
  List<EquipmentTrait> Traits { get; }
}
