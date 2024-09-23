using Logitar.Data;
using SkillCraft.Domain;

namespace SkillCraft.EntityFrameworkCore.SkillCraftDb;

internal static class Helper
{
  public static TableId GetTableId(EntityType entityType) => entityType switch
  {
    EntityType.Aspect => Aspects.Table,
    EntityType.Caste => Castes.Table,
    EntityType.Character => throw new NotImplementedException(), // TODO(fpion): implement
    EntityType.Customization => Customizations.Table,
    EntityType.Education => Educations.Table,
    EntityType.Language => Languages.Table,
    EntityType.Lineage => Lineages.Table,
    EntityType.Party => Parties.Table,
    EntityType.Personality => Personalities.Table,
    EntityType.Talent => Talents.Table,
    EntityType.World => Worlds.Table,
    _ => throw new ArgumentException($"The entity type '{entityType}' is not supported."),
  };

  public static string Normalize(Slug slug) => Normalize(slug.Value);
  public static string Normalize(string value) => value.Trim().ToUpperInvariant();
}
