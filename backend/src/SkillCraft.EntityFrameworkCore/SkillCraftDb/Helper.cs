using SkillCraft.Domain;

namespace SkillCraft.EntityFrameworkCore.SkillCraftDb;

internal static class Helper
{
  public static string Normalize(Slug slug) => Normalize(slug.Value);
  public static string Normalize(string value) => value.Trim().ToUpperInvariant();
}
