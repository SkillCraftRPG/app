namespace SkillCraft.Contracts.Characters;

public record CharacterAttributesModel
{
  public CharacterAttributeModel Agility { get; set; } = new();
  public CharacterAttributeModel Coordination { get; set; } = new();
  public CharacterAttributeModel Intellect { get; set; } = new();
  public CharacterAttributeModel Presence { get; set; } = new();
  public CharacterAttributeModel Sensitivity { get; set; } = new();
  public CharacterAttributeModel Spirit { get; set; } = new();
  public CharacterAttributeModel Vigor { get; set; } = new();

  public CharacterAttributesModel()
  {
  }

  public CharacterAttributesModel(CharacterModel character)
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

    scores[Attribute.Agility] += Math.Max(character.Lineage.Attributes.Agility, character.Lineage.Species?.Attributes.Agility ?? 0);
    scores[Attribute.Coordination] += Math.Max(character.Lineage.Attributes.Coordination, character.Lineage.Species?.Attributes.Coordination ?? 0);
    scores[Attribute.Intellect] += Math.Max(character.Lineage.Attributes.Intellect, character.Lineage.Species?.Attributes.Intellect ?? 0);
    scores[Attribute.Presence] += Math.Max(character.Lineage.Attributes.Presence, character.Lineage.Species?.Attributes.Presence ?? 0);
    scores[Attribute.Sensitivity] += Math.Max(character.Lineage.Attributes.Sensitivity, character.Lineage.Species?.Attributes.Sensitivity ?? 0);
    scores[Attribute.Spirit] += Math.Max(character.Lineage.Attributes.Spirit, character.Lineage.Species?.Attributes.Spirit ?? 0);
    scores[Attribute.Vigor] += Math.Max(character.Lineage.Attributes.Vigor, character.Lineage.Species?.Attributes.Vigor ?? 0);
    foreach (Attribute attribute in character.BaseAttributes.Extra)
    {
      scores[attribute] += 1;
    }

    if (character.Nature.Attribute.HasValue)
    {
      scores[character.Nature.Attribute.Value] += 1;
    }

    scores[character.BaseAttributes.Best] += 3;
    scores[character.BaseAttributes.Worst] += 1;
    foreach (Attribute attribute in character.BaseAttributes.Mandatory)
    {
      scores[attribute] += 2;
    }
    foreach (Attribute attribute in character.BaseAttributes.Optional)
    {
      scores[attribute] += 1;
    }

    foreach (BonusModel bonus in character.Bonuses)
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

    foreach (LevelUpModel levelUp in character.LevelUps)
    {
      scores[levelUp.Attribute] += 1;
    }

    foreach (KeyValuePair<Attribute, int> score in scores)
    {
      temporaryScores[score.Key] += score.Value;
    }

    Agility = new CharacterAttributeModel(scores[Attribute.Agility], temporaryScores[Attribute.Agility]);
    Coordination = new CharacterAttributeModel(scores[Attribute.Coordination], temporaryScores[Attribute.Coordination]);
    Intellect = new CharacterAttributeModel(scores[Attribute.Intellect], temporaryScores[Attribute.Intellect]);
    Presence = new CharacterAttributeModel(scores[Attribute.Presence], temporaryScores[Attribute.Presence]);
    Sensitivity = new CharacterAttributeModel(scores[Attribute.Sensitivity], temporaryScores[Attribute.Sensitivity]);
    Spirit = new CharacterAttributeModel(scores[Attribute.Spirit], temporaryScores[Attribute.Spirit]);
    Vigor = new CharacterAttributeModel(scores[Attribute.Vigor], temporaryScores[Attribute.Vigor]);
  }
}
