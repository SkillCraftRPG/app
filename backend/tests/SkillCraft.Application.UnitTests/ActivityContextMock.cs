using Logitar.Portal.Contracts.Users;

namespace SkillCraft.Application;

internal record ActivityContextMock : ActivityContext
{
  public ActivityContextMock(User? user = null) : base(ApiKey: null, Session: null, user ?? new UserMock(), World: null)
  {
  }

  public static void Contextualize(Activity activity)
  {
    ActivityContextMock context = new();
    activity.Contextualize(context);
  }
}
