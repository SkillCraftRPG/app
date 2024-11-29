using Logitar.Portal.Contracts.Users;

namespace SkillCraft.Domain.Worlds;

internal class WorldMock : World
{
  public WorldMock() : this(UserId.NewId())
  {
  }

  public WorldMock(User user) : this(new UserId(user.Id))
  {
  }

  public WorldMock(UserId ownerId) : base(new Slug("ungar"), ownerId)
  {
  }
}
