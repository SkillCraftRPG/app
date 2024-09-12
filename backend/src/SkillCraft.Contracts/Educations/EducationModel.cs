using Logitar.Portal.Contracts;

namespace SkillCraft.Contracts.Educations;

public class EducationModel : Aggregate
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public Skill? Skill { get; set; }
  public double? WealthMultiplier { get; set; }

  public EducationModel() : this(string.Empty)
  {
  }

  public EducationModel(string name)
  {
    Name = name;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
