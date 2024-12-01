namespace SkillCraft.Contracts.Characters;

public record CharacterStatisticsModel
{
  public CharacterStatisticModel Constitution { get; set; } = new();
  public CharacterStatisticModel Initiative { get; set; } = new();
  public CharacterStatisticModel Learning { get; set; } = new();
  public CharacterStatisticModel Power { get; set; } = new();
  public CharacterStatisticModel Precision { get; set; } = new();
  public CharacterStatisticModel Reputation { get; set; } = new();
  public CharacterStatisticModel Strength { get; set; } = new();

  public CharacterStatisticsModel()
  {
  }

  public CharacterStatisticsModel(CharacterModel character)
  {
    Dictionary<Statistic, int> bases = new()
    {
      [Statistic.Constitution] = 5 * (character.Attributes.Vigor.Modifier + 5),
      [Statistic.Initiative] = character.Attributes.Sensitivity.Modifier,
      [Statistic.Learning] = Math.Max((2 * character.Attributes.Intellect.Modifier) + 5, 5),
      [Statistic.Power] = character.Attributes.Spirit.Modifier,
      [Statistic.Precision] = character.Attributes.Coordination.Modifier,
      [Statistic.Reputation] = character.Attributes.Presence.Modifier,
      [Statistic.Strength] = character.Attributes.Agility.Modifier
    };

    Dictionary<Statistic, double> increments = new()
    {
      [Statistic.Constitution] = character.Attributes.Vigor.Modifier + 5,
      [Statistic.Initiative] = character.Attributes.Sensitivity.Score / 40.0,
      [Statistic.Learning] = Math.Max(character.Attributes.Intellect.Modifier + 2, 1),
      [Statistic.Power] = character.Attributes.Spirit.Score / 40.0,
      [Statistic.Precision] = character.Attributes.Coordination.Score / 40.0,
      [Statistic.Reputation] = character.Attributes.Presence.Score / 20.0,
      [Statistic.Strength] = character.Attributes.Agility.Score / 40.0
    };

    Dictionary<Statistic, double> values = bases.ToDictionary(x => x.Key, x => (double)x.Value);
    foreach (LevelUpModel levelUp in character.LevelUps)
    {
      values[Statistic.Constitution] += levelUp.Constitution;
      values[Statistic.Initiative] += levelUp.Initiative;
      values[Statistic.Learning] += levelUp.Learning;
      values[Statistic.Power] += levelUp.Power;
      values[Statistic.Precision] += levelUp.Precision;
      values[Statistic.Reputation] += levelUp.Reputation;
      values[Statistic.Strength] += levelUp.Strength;
    }
    foreach (BonusModel bonus in character.Bonuses)
    {
      if (bonus.Category == BonusCategory.Statistic && Enum.TryParse(bonus.Target, out Statistic statistic))
      {
        values[statistic] += bonus.Value;
      }
    }

    Constitution = new CharacterStatisticModel((int)Math.Floor(values[Statistic.Constitution]), bases[Statistic.Constitution], increments[Statistic.Constitution]);
    Initiative = new CharacterStatisticModel((int)Math.Floor(values[Statistic.Initiative]), bases[Statistic.Initiative], increments[Statistic.Initiative]);
    Learning = new CharacterStatisticModel((int)Math.Floor(values[Statistic.Learning]), bases[Statistic.Learning], increments[Statistic.Learning]);
    Power = new CharacterStatisticModel((int)Math.Floor(values[Statistic.Power]), bases[Statistic.Power], increments[Statistic.Power]);
    Precision = new CharacterStatisticModel((int)Math.Floor(values[Statistic.Precision]), bases[Statistic.Precision], increments[Statistic.Precision]);
    Reputation = new CharacterStatisticModel((int)Math.Floor(values[Statistic.Reputation]), bases[Statistic.Reputation], increments[Statistic.Reputation]);
    Strength = new CharacterStatisticModel((int)Math.Floor(values[Statistic.Strength]), bases[Statistic.Strength], increments[Statistic.Strength]);
  }
}
