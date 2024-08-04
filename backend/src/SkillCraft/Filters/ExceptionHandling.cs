using FluentValidation;
using FluentValidation.Results;
using Logitar.Net.Http;
using Logitar.Portal.Contracts.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SkillCraft.Application;
using SkillCraft.Application.Logging;
using SkillCraft.Contracts.Errors;

namespace SkillCraft.Filters;

internal class ExceptionHandling : ExceptionFilterAttribute
{
  private static readonly JsonSerializerOptions _serializerOptions = new()
  {
    PropertyNameCaseInsensitive = true
  };

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
    else if (context.Exception is ConflictException conflict)
    {
      context.Result = new ConflictObjectResult(conflict.Error);
      context.ExceptionHandled = true;
    }
    else if (context.Exception is HttpFailureException<JsonApiResult> portal) // ISSUE: https://github.com/SkillCraftRPG/app/issues/10
    {
      Error? error = null;
      try
      {
        error = portal.Result.Deserialize<Error>(_serializerOptions);
      }
      catch (Exception)
      {
      }

      context.Result = new JsonResult(new
      {
        portal.Message,
        portal.Result.JsonContent,
        Error = error
      }, _serializerOptions)
      {
        StatusCode = StatusCodes.Status500InternalServerError
      };
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
