using SkillCraft.Contracts.Parties;

namespace SkillCraft.Tools.Seeding.Backend;

internal record PartyPayload : CreateOrReplacePartyPayload
{
  public Guid Id { get; set; }
}
