using Logitar.Net.Http;
using SkillCraft.Application.Aspects.Commands;
using SkillCraft.Application.Castes.Commands;
using SkillCraft.Application.Customizations.Commands;
using SkillCraft.Application.Educations.Commands;
using SkillCraft.Application.Parties.Commands;
using SkillCraft.Application.Personalities.Commands;
using SkillCraft.Application.Worlds.Commands;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Contracts.Castes;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Contracts.Educations;
using SkillCraft.Contracts.Parties;
using SkillCraft.Contracts.Personalities;
using SkillCraft.Contracts.Worlds;

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

  public async Task<CustomizationModel> SaveCustomizationAsync(SaveCustomizationCommand command, CancellationToken cancellationToken)
  {
    Uri uri = new($"/customizations/{command.Id}?version={command.Version}", UriKind.Relative);
    JsonRequestOptions options = new(command.Payload)
    {
      SerializerOptions = _options
    };
    options.Headers.Add(new HttpHeader("X-World", "ungar")); // TODO(fpion): refactor
    JsonApiResult result = await _client.PutAsync(uri, options, cancellationToken);
    return result.Deserialize<CustomizationModel>(_options) ?? throw new InvalidOperationException("The customization could not be deserialized.");
  }

  public async Task<EducationModel> SaveEducationAsync(SaveEducationCommand command, CancellationToken cancellationToken)
  {
    Uri uri = new($"/educations/{command.Id}?version={command.Version}", UriKind.Relative);
    JsonRequestOptions options = new(command.Payload)
    {
      SerializerOptions = _options
    };
    options.Headers.Add(new HttpHeader("X-World", "ungar")); // TODO(fpion): refactor
    JsonApiResult result = await _client.PutAsync(uri, options, cancellationToken);
    return result.Deserialize<EducationModel>(_options) ?? throw new InvalidOperationException("The education could not be deserialized.");
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

  public async Task<PersonalityModel> SavePersonalityAsync(CreateOrReplacePersonalityCommand command, CancellationToken cancellationToken)
  {
    Uri uri = new($"/personalities/{command.Id}?version={command.Version}", UriKind.Relative);
    JsonRequestOptions options = new(command.Payload)
    {
      SerializerOptions = _options
    };
    options.Headers.Add(new HttpHeader("X-World", "ungar")); // TODO(fpion): refactor
    JsonApiResult result = await _client.PutAsync(uri, options, cancellationToken);
    return result.Deserialize<PersonalityModel>(_options) ?? throw new InvalidOperationException("The personality could not be deserialized.");
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
