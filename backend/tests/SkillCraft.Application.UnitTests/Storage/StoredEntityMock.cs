using SkillCraft.Domain;

namespace SkillCraft.Application.Storage;

internal class StoredEntityMock : IStoredEntity
{
  public int Size { get; }

  public StoredEntityMock(int size)
  {
    Size = size;
  }
}
