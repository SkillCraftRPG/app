using SkillCraft.Contracts;
using SkillCraft.Contracts.Parties;
using SkillCraft.Domain;
using SkillCraft.Domain.Parties;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Parties;

internal static class PartyExtensions
{
  private const EntityType Type = EntityType.Party;

  public static EntityMetadata GetMetadata(this Party party)
  {
    long size = party.Name.Size + (party.Description?.Size ?? 0);
    return new EntityMetadata(party.WorldId, new EntityKey(Type, party.EntityId), size);
  }
  public static EntityMetadata GetMetadata(this PartyModel party)
  {
    long size = party.Name.Length + (party.Description?.Length ?? 0);
    return new EntityMetadata(new WorldId(party.World.Id), new EntityKey(Type, party.Id), size);
  }
}
