using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SkillCraft.Application;
using SkillCraft.Application.Logging;
using SkillCraft.Contracts.Errors;

namespace SkillCraft.Filters;

internal class ExceptionHandling : ExceptionFilterAttribute
{
  private readonly ILoggingService _loggingService;

  public ExceptionHandling(ILoggingService loggingService)
  {
    _loggingService = loggingService;
  }

  public override void OnException(ExceptionContext context)
  {
    if (context.Exception is ValidationException validation)
    {
      ValidationError error = new();
      foreach (ValidationFailure failure in validation.Errors)
      {
        error.Add(new PropertyError(failure.ErrorCode, failure.ErrorMessage, failure.AttemptedValue, failure.PropertyName));
      }
      context.Result = new BadRequestObjectResult(error);
      context.ExceptionHandled = true;
    }
    else if (context.Exception is BadRequestException badRequest)
    {
      context.Result = new BadRequestObjectResult(badRequest.Error);
      context.ExceptionHandled = true;
    }
    else
    {
      base.OnException(context);
    }

    if (context.ExceptionHandled)
    {
      _loggingService.Report(context.Exception);
    }
  }
}
