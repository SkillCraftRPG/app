using Logitar.Identity.Contracts;

namespace SkillCraft.Contracts.Worlds;

public record UpdateWorldPayload
{
  public string? UniqueSlug { get; set; }
  public Modification<string>? DisplayName { get; set; }
  public Modification<string>? Description { get; set; }
}
