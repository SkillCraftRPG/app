using SkillCraft.Contracts;
using SkillCraft.Domain;
using SkillCraft.Domain.Languages;

namespace SkillCraft.Application.Languages;

internal static class LanguageExtensions
{
  public static EntityMetadata GetMetadata(this Language language) => new(language.WorldId, new EntityKey(EntityType.Language, language.EntityId), language.CalculateSize());

  private static long CalculateSize(this Language language) => language.Name.Size + (language.Description?.Size ?? 0)
    + (language.Script?.Size ?? 0) + (language.TypicalSpeakers?.Size ?? 0);
}
