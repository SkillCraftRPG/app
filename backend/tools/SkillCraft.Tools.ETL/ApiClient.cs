using Logitar.Net.Http;
using SkillCraft.Application.Aspects.Commands;
using SkillCraft.Application.Castes.Commands;
using SkillCraft.Application.Parties.Commands;
using SkillCraft.Application.Worlds.Commands;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Contracts.Castes;
using SkillCraft.Contracts.Parties;
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

  public async Task<AspectModel> SaveAspectAsync(SaveAspectCommand command, CancellationToken cancellationToken)
  {
    Uri uri = new($"/aspects/{command.Id}?version={command.Version}", UriKind.Relative);
    JsonRequestOptions options = new(command.Payload)
    {
      SerializerOptions = _options
    };
    options.Headers.Add(new HttpHeader("X-World", "ungar")); // TODO(fpion): refactor
    JsonApiResult result = await _client.PutAsync(uri, options, cancellationToken);
    return result.Deserialize<AspectModel>(_options) ?? throw new InvalidOperationException("The aspect could not be deserialized.");
  }

  public async Task<CasteModel> SaveCasteAsync(SaveCasteCommand command, CancellationToken cancellationToken)
  {
    Uri uri = new($"/castes/{command.Id}?version={command.Version}", UriKind.Relative);
    JsonRequestOptions options = new(command.Payload)
    {
      SerializerOptions = _options
    };
    options.Headers.Add(new HttpHeader("X-World", "ungar")); // TODO(fpion): refactor
    JsonApiResult result = await _client.PutAsync(uri, options, cancellationToken);
    return result.Deserialize<CasteModel>(_options) ?? throw new InvalidOperationException("The caste could not be deserialized.");
  }

  public async Task<PartyModel> SavePartyAsync(SavePartyCommand command, CancellationToken cancellationToken)
  {
    Uri uri = new($"/parties/{command.Id}?version={command.Version}", UriKind.Relative);
    JsonRequestOptions options = new(command.Payload)
    {
      SerializerOptions = _options
    };
    options.Headers.Add(new HttpHeader("X-World", "ungar")); // TODO(fpion): refactor
    JsonApiResult result = await _client.PutAsync(uri, options, cancellationToken);
    return result.Deserialize<PartyModel>(_options) ?? throw new InvalidOperationException("The party could not be deserialized.");
  }

  public async Task<WorldModel> SaveWorldAsync(SaveWorldCommand command, CancellationToken cancellationToken)
  {
    Uri uri = new($"/worlds/{command.Id}?version={command.Version}", UriKind.Relative);
    JsonRequestOptions options = new(command.Payload)
    {
      SerializerOptions = _options
    };
    JsonApiResult result = await _client.PutAsync(uri, options, cancellationToken);
    return result.Deserialize<WorldModel>(_options) ?? throw new InvalidOperationException("The world could not be deserialized.");
  }
}
