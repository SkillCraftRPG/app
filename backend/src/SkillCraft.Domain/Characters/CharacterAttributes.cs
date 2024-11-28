using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Lineages;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Domain.Characters;

public record CharacterAttributes
{
  public CharacterAttribute Agility { get; }
  public CharacterAttribute Coordination { get; }
  public CharacterAttribute Intellect { get; }
  public CharacterAttribute Presence { get; }
  public CharacterAttribute Sensitivity { get; }
  public CharacterAttribute Spirit { get; }
  public CharacterAttribute Vigor { get; }

  public CharacterAttributes(Character character)
    : this(character.BaseAttributes, character.LineageAttributes.Values, character.NatureAttribute, character.Bonuses.Values, character.LevelUps)
  {
  }

  public CharacterAttributes(
    BaseAttributes baseAttributes,
    IEnumerable<AttributeBonuses> lineageAttributes,
    Attribute? natureAttribute,
    IEnumerable<Bonus> bonuses,
    IEnumerable<LevelUp> levelUps)
  {
    Dictionary<Attribute, int> scores = new()
    {
      [Attribute.Agility] = baseAttributes.Agility,
      [Attribute.Coordination] = baseAttributes.Coordination,
      [Attribute.Intellect] = baseAttributes.Intellect,
      [Attribute.Presence] = baseAttributes.Presence,
      [Attribute.Sensitivity] = baseAttributes.Sensitivity,
      [Attribute.Spirit] = baseAttributes.Spirit,
      [Attribute.Vigor] = baseAttributes.Vigor
    };
    Dictionary<Attribute, int> temporaryScores = scores.ToDictionary(x => x.Key, x => 0);

    foreach (AttributeBonuses attributes in lineageAttributes)
    {
      scores[Attribute.Agility] += attributes.Agility;
      scores[Attribute.Coordination] += attributes.Coordination;
      scores[Attribute.Intellect] += attributes.Intellect;
      scores[Attribute.Presence] += attributes.Presence;
      scores[Attribute.Sensitivity] += attributes.Sensitivity;
      scores[Attribute.Spirit] += attributes.Spirit;
      scores[Attribute.Vigor] += attributes.Vigor;
    }
    foreach (Attribute extra in baseAttributes.Extra)
    {
      scores[extra] += 1;
    }

    if (natureAttribute.HasValue)
    {
      scores[natureAttribute.Value] += 1;
    }

    scores[baseAttributes.Best] += 3;
    scores[baseAttributes.Worst] += 1;
    foreach (Attribute mandatory in baseAttributes.Mandatory)
    {
      scores[mandatory] += 2;
    }
    foreach (Attribute optional in baseAttributes.Optional)
    {
      scores[optional] += 1;
    }

    foreach (Bonus bonus in bonuses)
    {
      if (bonus.Category == BonusCategory.Attribute && Enum.TryParse(bonus.Target, out Attribute attribute))
      {
        if (bonus.IsTemporary)
        {
          temporaryScores[attribute] += bonus.Value;
        }
        else
        {
          scores[attribute] += bonus.Value;
        }
      }
    }

    foreach (LevelUp levelUp in levelUps)
    {
      scores[levelUp.Attribute] += 1;
    }

    foreach (KeyValuePair<Attribute, int> score in scores)
    {
      temporaryScores[score.Key] += score.Value;
    }

    Agility = new CharacterAttribute(scores[Attribute.Agility], temporaryScores[Attribute.Agility]);
    Coordination = new CharacterAttribute(scores[Attribute.Coordination], temporaryScores[Attribute.Coordination]);
    Intellect = new CharacterAttribute(scores[Attribute.Intellect], temporaryScores[Attribute.Intellect]);
    Presence = new CharacterAttribute(scores[Attribute.Presence], temporaryScores[Attribute.Presence]);
    Sensitivity = new CharacterAttribute(scores[Attribute.Sensitivity], temporaryScores[Attribute.Sensitivity]);
    Spirit = new CharacterAttribute(scores[Attribute.Spirit], temporaryScores[Attribute.Spirit]);
    Vigor = new CharacterAttribute(scores[Attribute.Vigor], temporaryScores[Attribute.Vigor]);
  }
}
