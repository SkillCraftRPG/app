namespace SkillCraft.Domain.Lineages;

public record Weight
{
  public Roll? Starved { get; }
  public Roll? Skinny { get; }
  public Roll? Normal { get; }
  public Roll? Overweight { get; }
  public Roll? Obese { get; }

  [JsonIgnore]
  public int Size => (Starved?.Size ?? 0) + (Skinny?.Size ?? 0) + (Normal?.Size ?? 0) + (Overweight?.Size ?? 0) + (Obese?.Size ?? 0);

  public Weight() : this(null, null, null, null, null)
  {
  }

  [JsonConstructor]
  public Weight(Roll? starved, Roll? skinny, Roll? normal, Roll? overweight, Roll? obese)
  {
    Starved = starved;
    Skinny = skinny;
    Normal = normal;
    Overweight = overweight;
    Obese = obese;
  }
}
