﻿using Logitar.EventSourcing;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Application.Logging;

public interface ILoggingService
{
  void Open(string? correlationId = null, string? method = null, string? destination = null, string? source = null, string? additionalInformation = null, DateTime? startedOn = null);
  void Report(DomainEvent @event);
  void Report(Exception exception);
  void SetActivity(IActivity activity);
  void SetOperation(Operation operation);
  void SetApiKey(ApiKey? apiKey);
  void SetSession(Session? session);
  void SetUser(User? user);
  void SetWorld(WorldModel? world);
  Task CloseAndSaveAsync(int statusCode, CancellationToken cancellationToken = default);
}
