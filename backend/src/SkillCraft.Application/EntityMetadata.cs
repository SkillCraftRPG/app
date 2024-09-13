using SkillCraft.Contracts.Castes;
using SkillCraft.Contracts.Educations;
using SkillCraft.Domain;
using SkillCraft.Domain.Castes;
using SkillCraft.Domain.Educations;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application;

public record EntityMetadata
{
  public WorldId WorldId { get; }

  public EntityType Type { get; }
  public Guid Id { get; }
  public EntityKey Key { get; }

  public long Size { get; }

  private EntityMetadata(WorldId worldId, EntityType type, Guid id, long size)
  {
    WorldId = worldId;

    Type = type;
    Id = id;
    Key = new(type, id);

    Size = size;
  }

  public static EntityMetadata From(Caste caste) // TODO(fpion): refactor
  {
    long size = caste.Name.Value.Length + (caste.Description?.Value.Length ?? 0) + 4 + (caste.WealthRoll?.Value.Length ?? 0); // TODO(fpion): Traits
    return new EntityMetadata(caste.WorldId, EntityType.Education, caste.Id.ToGuid(), size);
  }
  public static EntityMetadata From(CasteModel caste) // TODO(fpion): refactor
  {
    long size = caste.Name.Length + (caste.Description?.Length ?? 0) + 4 + (caste.WealthRoll?.Length ?? 0); // TODO(fpion): Traits
    return new EntityMetadata(new WorldId(caste.World.Id), EntityType.Education, caste.Id, size);
  }

  public static EntityMetadata From(Education education) // TODO(fpion): refactor
  {
    long size = education.Name.Value.Length + (education.Description?.Value.Length ?? 0) + 4 + 8;
    return new EntityMetadata(education.WorldId, EntityType.Education, education.Id.ToGuid(), size);
  }
  public static EntityMetadata From(EducationModel education) // TODO(fpion): refactor
  {
    long size = education.Name.Length + (education.Description?.Length ?? 0) + 4 + 8;
    return new EntityMetadata(new WorldId(education.World.Id), EntityType.Education, education.Id, size);
  }

  public static EntityMetadata From(World world) // TODO(fpion): refactor
  {
    long size = world.Slug.Value.Length + (world.Name?.Value.Length ?? 0) + (world.Description?.Value.Length ?? 0);
    return new EntityMetadata(world.Id, EntityType.World, world.Id.ToGuid(), size);
  }
}
