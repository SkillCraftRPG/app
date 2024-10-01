using Logitar.Net.Http;
using SkillCraft.Application.Worlds.Commands;
using SkillCraft.Contracts.Worlds;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SkillCraft.Tools.ETL;

internal class ApiClient : IApiClient
{
  private readonly JsonApiClient _client;
  private readonly JsonSerializerOptions _options = new();

  public ApiClient(IHttpApiSettings settings)
  {
    _client = new JsonApiClient(settings);

    _options.Converters.Add(new JsonStringEnumConverter());
  }

  public async Task<WorldModel> SaveWorldAsync(SaveWorldCommand command, CancellationToken cancellationToken)
  {
    Uri uri = new($"/worlds/{command.Id}", UriKind.Relative);
    JsonRequestOptions options = new(command.Payload)
    {
      SerializerOptions = _options
    };
    JsonApiResult result = await _client.PutAsync(uri, options, cancellationToken);
    return result.Deserialize<WorldModel>(_options) ?? throw new InvalidOperationException("The world could not be deserialized.");
  }
}
