﻿using SkillCraft.Domain;

namespace SkillCraft.Constants;

internal static class Routes
{
  public const string Aspect = "aspects";
  public const string Caste = "castes";
  public const string Character = "characters";
  public const string Customization = "customizations";
  public const string Education = "educations";
  public const string Language = "languages";
  public const string Lineage = "lineages";
  public const string Personality = "personalities";
  public const string Talent = "talents";
  public const string World = "worlds";

  private static readonly Dictionary<string, EntityType> _entityTypes = new()
  {
    [Aspect] = EntityType.Aspect,
    [Caste] = EntityType.Caste,
    [Character] = EntityType.Character,
    [Customization] = EntityType.Customization,
    [Education] = EntityType.Education,
    [Language] = EntityType.Language,
    [Lineage] = EntityType.Lineage,
    [Personality] = EntityType.Personality,
    [Talent] = EntityType.Talent,
    [World] = EntityType.World
  };
  public static EntityType? GetEntityType(string route)
  {
    return _entityTypes.TryGetValue(route.Trim().ToLowerInvariant(), out EntityType entityType) ? entityType : null;
  }
}