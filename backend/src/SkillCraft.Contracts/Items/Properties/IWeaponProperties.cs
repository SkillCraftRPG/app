namespace SkillCraft.Contracts.Items.Properties;

public interface IWeaponProperties
{
  int? Attack { get; } // TODO(fpion): nullability
  string? DamageRoll { get; } // TODO(fpion): nullability
  // TODO(fpion): DamageType
  int? Resistance { get; } // TODO(fpion): nullability
  // TODO(fpion): Properties
}
