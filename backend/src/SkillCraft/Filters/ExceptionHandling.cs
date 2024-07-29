using Microsoft.AspNetCore.Mvc.Filters;

namespace SkillCraft.Filters;

internal class ExceptionHandling : ExceptionFilterAttribute
{
  public override void OnException(ExceptionContext context)
  {
    base.OnException(context);
  }
}
