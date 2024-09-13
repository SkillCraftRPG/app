using Logitar.EventSourcing;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using SkillCraft.Application.Caching;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Application.Logging;

public class LoggingService : ILoggingService
{
  private Log? _log = null;

  private readonly ICacheService _cacheService;
  private readonly ILogRepository _logRepository;
  private readonly ILoggingSettings _settings;

  public LoggingService(ICacheService cacheService, ILogRepository logRepository, ILoggingSettings settings)
  {
    _cacheService = cacheService;
    _logRepository = logRepository;
    _settings = settings;
  }

  public void Open(string? correlationId, string? method, string? destination, string? source, string? additionalInformation, DateTime? startedOn)
  {
    if (_log != null)
    {
      throw new InvalidOperationException($"You must close the current log by calling one of the '{nameof(CloseAndSaveAsync)}' methods before opening a new log.");
    }

    _log = Log.Open(correlationId, method, destination, source, additionalInformation);
  }

  public void Report(DomainEvent @event)
  {
    _log?.Report(@event);
  }

  public void Report(Exception exception)
  {
    _log?.Report(exception);
  }

  public void SetActivity(IActivity activity)
  {
    _log?.SetActivity(activity);
  }

  public void SetOperation(Operation operation)
  {
    _log?.SetOperation(operation);
  }

  public void SetApiKey(ApiKey? apiKey)
  {
    if (_log != null)
    {
      _log.ApiKeyId = apiKey?.Id;
    }
  }

  public void SetSession(Session? session)
  {
    if (_log != null)
    {
      _log.SessionId = session?.Id;
    }
  }

  public void SetUser(User? user)
  {
    if (_log != null)
    {
      _log.UserId = user?.Id;
    }
  }

  public void SetWorld(WorldModel? world)
  {
    if (_log != null)
    {
      _log.WorldId = world?.Id;
    }
  }

  public async Task CloseAndSaveAsync(int statusCode, CancellationToken cancellationToken)
  {
    if (_log == null)
    {
      throw new InvalidOperationException($"You must open a new log by calling one of the '{nameof(Open)}' methods before calling the current method.");
    }

    _log.Close(statusCode);

    if (ShouldSaveLog())
    {
      await _logRepository.SaveAsync(_log, cancellationToken);
    }

    _log = null;
  }

  private bool ShouldSaveLog()
  {
    if (_log != null)
    {
      if (!_settings.OnlyErrors || _log.HasErrors)
      {
        switch (_settings.Extent)
        {
          case LoggingExtent.ActivityOnly:
            return _log.Activity != null;
          case LoggingExtent.Full:
            return true;
        }
      }
    }

    return false;
  }
}
