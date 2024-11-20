using SkillCraft.Contracts;

namespace SkillCraft.Constants;

internal static class Routes
{
  public const string Aspect = "aspects";
  public const string Caste = "castes";
  public const string Character = "characters";
  public const string Comment = "comments";
  public const string Customization = "customizations";
  public const string Education = "educations";
  public const string Item = "items";
  public const string Language = "languages";
  public const string Lineage = "lineages";
  public const string Nature = "natures";
  public const string Party = "parties";
  public const string Talent = "talents";
  public const string World = "worlds";

  private static readonly Dictionary<string, EntityType> _entityTypes = new()
  {
    [Aspect] = EntityType.Aspect,
    [Caste] = EntityType.Caste,
    [Character] = EntityType.Character,
    [Comment] = EntityType.Comment,
    [Customization] = EntityType.Customization,
    [Education] = EntityType.Education,
    [Item] = EntityType.Item,
    [Language] = EntityType.Language,
    [Lineage] = EntityType.Lineage,
    [Nature] = EntityType.Nature,
    [Party] = EntityType.Party,
    [Talent] = EntityType.Talent,
    [World] = EntityType.World
  };
  public static EntityType? GetEntityType(string route)
  {
    return _entityTypes.TryGetValue(route.Trim().ToLowerInvariant(), out EntityType entityType) ? entityType : null;
  }
}
