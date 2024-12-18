﻿using FluentValidation;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Characters.Validators;

namespace SkillCraft.Domain.Characters;

public record Bonus : IBonus
{
  public BonusCategory Category { get; }
  public string Target { get; }
  public int Value { get; }

  public bool IsTemporary { get; }
  public Name? Precision { get; }
  public Description? Notes { get; }

  public Bonus(BonusCategory category, string target, int value, bool isTemporary = false, Name? precision = null, Description? notes = null)
  {
    Category = category;
    Target = target;
    Value = value;

    IsTemporary = isTemporary;
    Precision = precision;
    Notes = notes;

    new BonusValidator().ValidateAndThrow(this);
  }
}
