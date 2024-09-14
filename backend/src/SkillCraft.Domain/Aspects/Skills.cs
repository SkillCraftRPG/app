using FluentValidation;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Domain.Aspects.Validators;

namespace SkillCraft.Domain.Aspects;

public record Skills : ISkills
{
  public Skill? Discounted1 { get; }
  public Skill? Discounted2 { get; }

  public Skills(Skill? discounted1, Skill? discounted2)
  {
    Discounted1 = discounted1;
    Discounted2 = discounted2;
    new SkillsValidator().ValidateAndThrow(this);
  }
}
