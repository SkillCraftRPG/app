using FluentValidation;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain.Lineages.Validators;

namespace SkillCraft.Domain.Lineages;

public record Ages : IAges
{
  public int? Adolescent { get; }
  public int? Adult { get; }
  public int? Mature { get; }
  public int? Venerable { get; }

  public Ages() : this(null, null, null, null)
  {
  }

  public Ages(IAges ages) : this(ages.Adolescent, ages.Adult, ages.Mature, ages.Venerable)
  {
  }

  [JsonConstructor]
  public Ages(int? adolescent, int? adult, int? mature, int? venerable)
  {
    Adolescent = adolescent;
    Adult = adult;
    Mature = mature;
    Venerable = venerable;
    new AgesValidator().ValidateAndThrow(this);
  }
}
