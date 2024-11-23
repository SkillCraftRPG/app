using SkillCraft.Contracts.Characters;

namespace SkillCraft.Tools.Seeding.Backend;

public record CharacterPayload : CreateCharacterPayload
{
  public Guid Id { get; set; }
}
