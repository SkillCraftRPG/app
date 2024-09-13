using Microsoft.OpenApi.Models;
using SkillCraft.Constants;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SkillCraft.Extensions;

internal class AddHeaderParameters : IOperationFilter
{
  public void Apply(OpenApiOperation operation, OperationFilterContext context)
  {
    operation.Parameters.Add(new OpenApiParameter
    {
      In = ParameterLocation.Header,
      Name = ApiHeaders.World,
      Description = "Enter your world ID or slug in the input below."
    });
  }
}
