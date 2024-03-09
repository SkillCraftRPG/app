using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SkillCraft.Contracts.Errors;
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
    else if (context.Exception is InvalidCredentialsException)
    {
      Error error = new(code: "InvalidCredentials", InvalidCredentialsException.ErrorMessage);
      context.Result = new BadRequestObjectResult(error);
      context.ExceptionHandled = true;
    }
    else if (context.Exception is NotImplementedException || context.Exception is NotSupportedException)
    {
      context.Result = new StatusCodeResult(StatusCodes.Status501NotImplemented);
      context.ExceptionHandled = true;
    }
    else
    {
      base.OnException(context);
    }
  }
}

// ISSUE #4: Portal Exception Handling
