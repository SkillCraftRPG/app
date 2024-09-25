using SkillCraft.Contracts;
using SkillCraft.Contracts.Languages;
using SkillCraft.Domain;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Languages;

internal static class LanguageExtensions
{
  private const EntityType Type = EntityType.Language;

  public static EntityMetadata GetMetadata(this Language language)
  {
    long size = language.Name.Size + (language.Description?.Size ?? 0) + (language.Script?.Size ?? 0) + (language.TypicalSpeakers?.Size ?? 0);
    return new EntityMetadata(language.WorldId, new EntityKey(Type, language.Id.ToGuid()), size);
  }
  public static EntityMetadata GetMetadata(this LanguageModel language)
  {
    long size = language.Name.Length + (language.Description?.Length ?? 0) + (language.Script?.Length ?? 0) + (language.TypicalSpeakers?.Length ?? 0);
    return new EntityMetadata(new WorldId(language.World.Id), new EntityKey(Type, language.Id), size);
  }
}
