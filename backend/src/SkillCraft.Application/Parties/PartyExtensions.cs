using SkillCraft.Contracts;
using SkillCraft.Domain;
using SkillCraft.Domain.Parties;

namespace SkillCraft.Application.Parties;

internal static class PartyExtensions
{
  public static EntityMetadata GetMetadata(this Party party) => new(party.WorldId, new EntityKey(EntityType.Party, party.EntityId), party.CalculateSize());

  private static long CalculateSize(this Party party) => party.Name.Size + (party.Description?.Size ?? 0);
}
