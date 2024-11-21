using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Characters;

public record SearchCharactersPayload : SearchPayload
{
  public string? PlayerName { get; set; }

  public new List<CharacterSortOption> Sort { get; set; } = [];
}
