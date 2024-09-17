using GraphQL;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Application;
using SkillCraft.Application.Logging;

namespace SkillCraft.GraphQL;

internal static class ResolveFieldContextExtensions
{
  public static async Task<T> ExecuteQueryAsync<T>(this IResolveFieldContext context, IRequest<T> query, CancellationToken cancellationToken)
  {
    ILoggingService loggingService = context.GetRequiredService<ILoggingService>();
    loggingService.SetOperation(new Operation("query", context.FieldDefinition.Name));

    IRequestPipeline pipeline = context.GetRequiredService<IRequestPipeline>();
    return await pipeline.ExecuteAsync(query, cancellationToken);
  }

  private static T GetRequiredService<T>(this IResolveFieldContext context) where T : notnull
  {
    if (context.RequestServices == null)
    {
      throw new ArgumentException($"The {nameof(context.RequestServices)} is required.", nameof(context));
    }

    return context.RequestServices.GetRequiredService<T>();
  }
}
