using MediatR;
using SkillCraft.Application;
using SkillCraft.Application.Characters;
using SkillCraft.Application.Characters.Commands;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Worlds;
using ActivityContext = SkillCraft.Application.ActivityContext;

namespace SkillCraft.Tools.Seeding.Backend.Tasks;

internal class SeedCharactersTask : SeedingTask
{
  public override string? Description => "Seeds the characters.";
}

internal class SeedCharactersTaskHandler : INotificationHandler<SeedCharactersTask>
{
  private static readonly JsonSerializerOptions _serializerOptions = new();
  static SeedCharactersTaskHandler()
  {
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  private readonly IActivityContextResolver _activityContextResolver;
  private readonly ICharacterQuerier _characterQuerier;
  private readonly ILogger<SeedCharactersTaskHandler> _logger;
  private readonly IRequestPipeline _pipeline;

  public SeedCharactersTaskHandler(
    IActivityContextResolver activityContextResolver,
    ICharacterQuerier characterQuerier,
    ILogger<SeedCharactersTaskHandler> logger,
    IRequestPipeline pipeline)
  {
    _activityContextResolver = activityContextResolver;
    _characterQuerier = characterQuerier;
    _logger = logger;
    _pipeline = pipeline;
  }

  public async Task Handle(SeedCharactersTask _, CancellationToken cancellationToken)
  {
    ActivityContext context = await _activityContextResolver.ResolveAsync(cancellationToken);
    if (context.World == null)
    {
      throw new InvalidOperationException("The world is required.");
    }
    WorldId worldId = new(context.World.Id);

    string json = await File.ReadAllTextAsync("Backend/characters.json", Encoding.UTF8, cancellationToken);
    IEnumerable<CharacterPayload>? payloads = JsonSerializer.Deserialize<IEnumerable<CharacterPayload>>(json, _serializerOptions);
    if (payloads != null)
    {
      IEnumerable<Guid> characterIds = payloads.Select(p => p.Id).Distinct();
      HashSet<Guid> existingIds = (await _characterQuerier.FindExistingAsync(worldId, characterIds, cancellationToken)).ToHashSet();

      foreach (CharacterPayload payload in payloads)
      {
        if (existingIds.Contains(payload.Id))
        {
          _logger.LogInformation("The character '{Name}' already exists (Id={Id}).", payload.Name, payload.Id);
        }
        else
        {
          CreateCharacterCommand command = new(payload.Id, payload);
          CharacterModel character = await _pipeline.ExecuteAsync(command, cancellationToken);
          _logger.LogInformation("The character '{Name}' has been created (Id={Id}).", character.Name, character.Id);
        }
      }
    }
  }
}
