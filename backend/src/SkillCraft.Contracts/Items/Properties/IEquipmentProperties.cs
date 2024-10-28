namespace SkillCraft.Contracts.Items.Properties;

public interface IEquipmentProperties
{
  int Defense { get; }
  int? Resistance { get; }
  EquipmentTrait[] Traits { get; }
}
