using MediatR;
using SkillCraft.Application;
using SkillCraft.Application.Parties.Commands;
using SkillCraft.Contracts.Parties;

namespace SkillCraft.Tools.Seeding.Backend.Tasks;

internal class SeedPartiesTask : SeedingTask
{
  public override string? Description => "Seeds the character parties.";
}

internal class SeedPartiesTaskHandler : INotificationHandler<SeedPartiesTask>
{
  private static readonly JsonSerializerOptions _serializerOptions = new();
  static SeedPartiesTaskHandler()
  {
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  private readonly ILogger<SeedPartiesTaskHandler> _logger;
  private readonly IRequestPipeline _pipeline;

  public SeedPartiesTaskHandler(ILogger<SeedPartiesTaskHandler> logger, IRequestPipeline pipeline)
  {
    _logger = logger;
    _pipeline = pipeline;
  }

  public async Task Handle(SeedPartiesTask _, CancellationToken cancellationToken)
  {
    string json = await File.ReadAllTextAsync("Backend/parties.json", Encoding.UTF8, cancellationToken);
    IEnumerable<PartyPayload>? payloads = JsonSerializer.Deserialize<IEnumerable<PartyPayload>>(json, _serializerOptions);
    if (payloads != null)
    {
      foreach (PartyPayload payload in payloads)
      {
        CreateOrReplacePartyCommand command = new(payload.Id, payload, Version: null);
        CreateOrReplacePartyResult result = await _pipeline.ExecuteAsync(command, cancellationToken);
        PartyModel party = result.Party ?? throw new InvalidOperationException("The party model should not be null.");
        string status = result.Created ? "created" : "updated";
        _logger.LogInformation("The party '{Name}' has been {Status} (Id={Id}).", party.Name, status, party.Id);
      }
    }
  }
}
