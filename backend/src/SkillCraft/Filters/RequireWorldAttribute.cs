using Logitar.Portal.Contracts.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SkillCraft.Constants;
using SkillCraft.Extensions;

namespace SkillCraft.Filters;

internal class RequireWorldAttribute : ActionFilterAttribute
{
  public override void OnActionExecuting(ActionExecutingContext context)
  {
    if (context.HttpContext.GetWorld() == null)
    {
      context.Result = new BadRequestObjectResult(new Error("WorldHeaderMissing", $"The '{ApiHeaders.World}' header is required."));
    }
    else
    {
      base.OnActionExecuting(context);
    }
  }
}
