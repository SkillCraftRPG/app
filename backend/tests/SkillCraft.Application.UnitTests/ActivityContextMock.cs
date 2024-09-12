using Logitar.Portal.Contracts.Users;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Application;

internal record ActivityContextMock : ActivityContext
{
  public ActivityContextMock(User? user = null, WorldModel? world = null)
    : base(ApiKey: null, Session: null, user ?? new UserMock(), world)
  {
  }
}
