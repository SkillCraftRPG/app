using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SkillCraft.Models.Errors;

namespace SkillCraft.Filters;

internal class ExceptionHandling : ExceptionFilterAttribute
{
  public override void OnException(ExceptionContext context)
  {
    if (context.Exception is ValidationException validation)
    {
      context.Result = new BadRequestObjectResult(new ValidationError(validation));
      context.ExceptionHandled = true;
    }
    else
    {
      base.OnException(context);
    }
  }
}

// TODO(fpion): handle exceptions thrown by the SkillCraft system
// TODO(fpion): handle exceptions thrown by the Portal system
