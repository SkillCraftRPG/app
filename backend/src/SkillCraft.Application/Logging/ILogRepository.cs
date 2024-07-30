﻿namespace SkillCraft.Application.Logging;

public interface ILogRepository
{
  Task SaveAsync(Log log, CancellationToken cancellationToken = default);
}
