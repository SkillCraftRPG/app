﻿using FluentValidation;
using SkillCraft.Contracts.Items.Properties;

namespace SkillCraft.Domain.Items.Properties;

public class EquipmentPropertiesValidator : AbstractValidator<IEquipmentProperties>
{
  public EquipmentPropertiesValidator()
  {
  }
}
