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

  public CharacterAttributes(IEnumerable<KeyValuePair<Attribute, int>> scores, IEnumerable<KeyValuePair<Attribute, int>>? temporaryScores = null)
  {
    Dictionary<Attribute, int> scoresDict = scores.GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Last().Value);
    Dictionary<Attribute, int> temporaryScoresDict = temporaryScores?.GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Last().Value) ?? [];

    Agility = Compile(Attribute.Agility, scoresDict, temporaryScoresDict);
    Coordination = Compile(Attribute.Coordination, scoresDict, temporaryScoresDict);
    Intellect = Compile(Attribute.Intellect, scoresDict, temporaryScoresDict);
    Presence = Compile(Attribute.Presence, scoresDict, temporaryScoresDict);
    Sensitivity = Compile(Attribute.Sensitivity, scoresDict, temporaryScoresDict);
    Spirit = Compile(Attribute.Spirit, scoresDict, temporaryScoresDict);
    Vigor = Compile(Attribute.Vigor, scoresDict, temporaryScoresDict);
  }

  private static CharacterAttribute Compile(Attribute attribute, IReadOnlyDictionary<Attribute, int> scores, IReadOnlyDictionary<Attribute, int> temporaryScores)
  {
    return new CharacterAttribute(
      scores.TryGetValue(attribute, out int score) ? score : 0,
      temporaryScores.TryGetValue(attribute, out int temporaryScore) ? temporaryScore : null);
  }
}
