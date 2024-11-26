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
  {
    Dictionary<Attribute, int> scores = new()
    {
      [Attribute.Agility] = character.BaseAttributes.Agility,
      [Attribute.Coordination] = character.BaseAttributes.Coordination,
      [Attribute.Intellect] = character.BaseAttributes.Intellect,
      [Attribute.Presence] = character.BaseAttributes.Presence,
      [Attribute.Sensitivity] = character.BaseAttributes.Sensitivity,
      [Attribute.Spirit] = character.BaseAttributes.Spirit,
      [Attribute.Vigor] = character.BaseAttributes.Vigor
    };
    Dictionary<Attribute, int> temporaryScores = scores.ToDictionary(x => x.Key, x => 0);

    foreach (AttributeBonuses bonuses in character.LineageAttributes.Values)
    {
      scores[Attribute.Agility] += bonuses.Agility;
      scores[Attribute.Coordination] += bonuses.Coordination;
      scores[Attribute.Intellect] += bonuses.Intellect;
      scores[Attribute.Presence] += bonuses.Presence;
      scores[Attribute.Sensitivity] += bonuses.Sensitivity;
      scores[Attribute.Spirit] += bonuses.Spirit;
      scores[Attribute.Vigor] += bonuses.Vigor;
    }
    foreach (Attribute extra in character.BaseAttributes.Extra)
    {
      scores[extra] += 1;
    }

    if (character.NatureAttribute.HasValue)
    {
      scores[character.NatureAttribute.Value] += 1;
    }

    scores[character.BaseAttributes.Best] += 3;
    scores[character.BaseAttributes.Worst] += 1;
    foreach (Attribute mandatory in character.BaseAttributes.Mandatory)
    {
      scores[mandatory] += 2;
    }
    foreach (Attribute optional in character.BaseAttributes.Optional)
    {
      scores[optional] += 1;
    }

    foreach (Bonus bonus in character.Bonuses.Values)
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

    foreach (LevelUp levelUp in character.LevelUps)
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
