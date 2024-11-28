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

    Dictionary<Statistic, double> sums = bases.ToDictionary(x => x.Key, x => 0.0);
    foreach (LevelUp levelUp in levelUps)
    {
      sums[Statistic.Constitution] += levelUp.Constitution;
      sums[Statistic.Initiative] += levelUp.Initiative;
      sums[Statistic.Learning] += levelUp.Learning;
      sums[Statistic.Power] += levelUp.Power;
      sums[Statistic.Precision] += levelUp.Precision;
      sums[Statistic.Reputation] += levelUp.Reputation;
      sums[Statistic.Strength] += levelUp.Strength;
    }

    Dictionary<Statistic, int> statisticBonuses = bases.ToDictionary(x => x.Key, x => 0);
    foreach (Bonus bonus in bonuses)
    {
      if (bonus.Category == BonusCategory.Statistic && Enum.TryParse(bonus.Target, out Statistic statistic))
      {
        statisticBonuses[statistic] += bonus.Value;
      }
    }

    Constitution = new CharacterStatistic(bases[Statistic.Constitution], increments[Statistic.Constitution], sums[Statistic.Constitution], statisticBonuses[Statistic.Constitution]);
    Initiative = new CharacterStatistic(bases[Statistic.Initiative], increments[Statistic.Initiative], sums[Statistic.Initiative], statisticBonuses[Statistic.Initiative]);
    Learning = new CharacterStatistic(bases[Statistic.Learning], increments[Statistic.Learning], sums[Statistic.Learning], statisticBonuses[Statistic.Learning]);
    Power = new CharacterStatistic(bases[Statistic.Power], increments[Statistic.Power], sums[Statistic.Power], statisticBonuses[Statistic.Power]);
    Precision = new CharacterStatistic(bases[Statistic.Precision], increments[Statistic.Precision], sums[Statistic.Precision], statisticBonuses[Statistic.Precision]);
    Reputation = new CharacterStatistic(bases[Statistic.Reputation], increments[Statistic.Reputation], sums[Statistic.Reputation], statisticBonuses[Statistic.Reputation]);
    Strength = new CharacterStatistic(bases[Statistic.Strength], increments[Statistic.Strength], sums[Statistic.Strength], statisticBonuses[Statistic.Strength]);
  }
}
