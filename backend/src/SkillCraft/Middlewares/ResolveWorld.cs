using Microsoft.Extensions.Primitives;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Worlds;
using SkillCraft.Constants;
using SkillCraft.Contracts.Errors;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Extensions;

namespace SkillCraft.Middlewares;

internal class ResolveWorld
{
  private readonly RequestDelegate _next;

  public ResolveWorld(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context, IPermissionService permissionService, IWorldQuerier worldQuerier)
  {
    HttpRequest request = context.Request;
    HttpResponse response = context.Response;

    if (request.Headers.TryGetValue(ApiHeaders.World, out StringValues headerValues))
    {
      IReadOnlyCollection<string> values = headerValues.GetNotNullOrWhiteSpace();
      if (values.Count > 1)
      {
        response.StatusCode = StatusCodes.Status400BadRequest;
        await response.WriteAsJsonAsync(new PropertyError("InvalidWorldHeader", "The world header only supports one value.", values, ApiHeaders.World));
        return;
      }
      else if (values.Count == 1)
      {
        string idOrSlug = values.Single();

        WorldModel? world = null;
        if (Guid.TryParse(idOrSlug, out Guid id))
        {
          world = await worldQuerier.ReadAsync(id);
        }
        world ??= await worldQuerier.ReadAsync(idOrSlug);

        if (world == null)
        {
          response.StatusCode = StatusCodes.Status404NotFound;
          await response.WriteAsJsonAsync(new PropertyError("WorldNotFound", "The specified world could not be found.", idOrSlug, ApiHeaders.World));
          return;
        }

        context.SetWorld(world);
      }
    }

    await _next(context);
  }
}
