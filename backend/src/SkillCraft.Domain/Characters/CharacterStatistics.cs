using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;

namespace SkillCraft.Domain.Characters;

public record CharacterStatistics
{
  public CharacterStatistic Constitution { get; }
  public CharacterStatistic Initiative { get; }
  public CharacterStatistic Learning { get; }
  public CharacterStatistic Power { get; }
  public CharacterStatistic Precision { get; }
  public CharacterStatistic Reputation { get; }
  public CharacterStatistic Strength { get; }

  public CharacterStatistics(Character character) : this(character.Attributes, character.LevelUps, character.Bonuses.Values)
  {
  }

  public CharacterStatistics(CharacterAttributes attributes, IEnumerable<LevelUp> levelUps, IEnumerable<Bonus> bonuses)
  {
    Dictionary<Statistic, int> bases = new()
    {
      [Statistic.Constitution] = 5 * (attributes.Vigor.Modifier + 5),
      [Statistic.Initiative] = attributes.Sensitivity.Modifier,
      [Statistic.Learning] = Math.Max((2 * attributes.Intellect.Modifier) + 5, 5),
      [Statistic.Power] = attributes.Spirit.Modifier,
      [Statistic.Precision] = attributes.Coordination.Modifier,
      [Statistic.Reputation] = attributes.Presence.Modifier,
      [Statistic.Strength] = attributes.Agility.Modifier
    };

    Dictionary<Statistic, double> increments = new()
    {
      [Statistic.Constitution] = attributes.Vigor.Modifier + 5,
      [Statistic.Initiative] = attributes.Sensitivity.Score / 40.0,
      [Statistic.Learning] = Math.Max(attributes.Intellect.Modifier + 2, 1),
      [Statistic.Power] = attributes.Spirit.Score / 40.0,
      [Statistic.Precision] = attributes.Coordination.Score / 40.0,
      [Statistic.Reputation] = attributes.Presence.Score / 20.0,
      [Statistic.Strength] = attributes.Agility.Score / 40.0
    };

    Dictionary<Statistic, double> values = bases.ToDictionary(x => x.Key, x => (double)x.Value);
    foreach (LevelUp levelUp in levelUps)
    {
      values[Statistic.Constitution] += levelUp.Constitution;
      values[Statistic.Initiative] += levelUp.Initiative;
      values[Statistic.Learning] += levelUp.Learning;
      values[Statistic.Power] += levelUp.Power;
      values[Statistic.Precision] += levelUp.Precision;
      values[Statistic.Reputation] += levelUp.Reputation;
      values[Statistic.Strength] += levelUp.Strength;
    }
    foreach (Bonus bonus in bonuses)
    {
      if (bonus.Category == BonusCategory.Statistic && Enum.TryParse(bonus.Target, out Statistic statistic))
      {
        values[statistic] += bonus.Value;
      }
    }

    Constitution = new CharacterStatistic((int)Math.Floor(values[Statistic.Constitution]), bases[Statistic.Constitution], increments[Statistic.Constitution]);
    Initiative = new CharacterStatistic((int)Math.Floor(values[Statistic.Initiative]), bases[Statistic.Initiative], increments[Statistic.Initiative]);
    Learning = new CharacterStatistic((int)Math.Floor(values[Statistic.Learning]), bases[Statistic.Learning], increments[Statistic.Learning]);
    Power = new CharacterStatistic((int)Math.Floor(values[Statistic.Power]), bases[Statistic.Power], increments[Statistic.Power]);
    Precision = new CharacterStatistic((int)Math.Floor(values[Statistic.Precision]), bases[Statistic.Precision], increments[Statistic.Precision]);
    Reputation = new CharacterStatistic((int)Math.Floor(values[Statistic.Reputation]), bases[Statistic.Reputation], increments[Statistic.Reputation]);
    Strength = new CharacterStatistic((int)Math.Floor(values[Statistic.Strength]), bases[Statistic.Strength], increments[Statistic.Strength]);
  }
}
