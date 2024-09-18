namespace SkillCraft.Contracts.Lineages;

public record WeightModel
{
  public string? Starved { get; set; }
  public string? Skinny { get; set; }
  public string? Normal { get; set; }
  public string? Overweight { get; set; }
  public string? Obese { get; set; }
}
