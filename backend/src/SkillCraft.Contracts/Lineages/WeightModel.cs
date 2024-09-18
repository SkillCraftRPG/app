namespace SkillCraft.Contracts.Lineages;

public record WeightModel
{
  public string Starved { get; set; }
  public string Skinny { get; set; }
  public string Normal { get; set; }
  public string Overweight { get; set; }
  public string Obese { get; set; }

  public WeightModel() : this(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty)
  {
  }

  public WeightModel(string starved, string skinny, string normal, string overweight, string obese)
  {
    Starved = starved;
    Skinny = skinny;
    Normal = normal;
    Overweight = overweight;
    Obese = obese;
  }
}
