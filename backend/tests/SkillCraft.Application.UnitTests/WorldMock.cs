using Logitar.Portal.Contracts.Users;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application;

internal class WorldMock : World
{
  public WorldMock() : this(new UserMock())
  {
  }

  public WorldMock(User user) : this(new UserId(user.Id))
  {
  }

  public WorldMock(UserId ownerId) : base(new Slug("ungar"), ownerId)
  {
  }
}
