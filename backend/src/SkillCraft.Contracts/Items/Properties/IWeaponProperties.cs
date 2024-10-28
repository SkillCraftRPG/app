﻿namespace SkillCraft.Contracts.Items.Properties;

public interface IWeaponProperties
{
  int Attack { get; }
  int? Resistance { get; }
  List<WeaponTrait> Traits { get; }

  List<WeaponDamageModel> Damages { get; }
  List<WeaponDamageModel> VersatileDamages { get; }

  WeaponRangeModel? Range { get; }
  int? ReloadCount { get; }
}
