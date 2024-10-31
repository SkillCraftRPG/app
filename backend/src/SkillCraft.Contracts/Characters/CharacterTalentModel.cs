using SkillCraft.Contracts.Talents;
using System.Text;

namespace SkillCraft.Contracts.Characters;

public class CharacterTalentModel
{
  public Guid Id { get; set; }

  public TalentModel Talent { get; set; }

  public int Cost { get; set; }
  public string? Precision { get; set; }
  public string? Notes { get; set; }

  public CharacterTalentModel() : this(new TalentModel())
  {
  }

  public CharacterTalentModel(TalentModel talent)
  {
    Talent = talent;
  }

  public override bool Equals(object? obj) => obj is CharacterTalentModel other && other.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString()
  {
    StringBuilder s = new();

    s.Append(Talent.Name);
    if (Precision != null)
    {
      s.Append(" (").Append(Precision).Append(')');
    }
    s.Append(" | ").Append(GetType()).Append(" (Id=").Append(Id).Append(')');

    return s.ToString();
  }
}
