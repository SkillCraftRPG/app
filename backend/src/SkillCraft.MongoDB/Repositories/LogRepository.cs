using Logitar;
using MongoDB.Driver;
using SkillCraft.Application.Logging;
using SkillCraft.Infrastructure;
using SkillCraft.MongoDB.Entities;

namespace SkillCraft.MongoDB.Repositories;

internal class LogRepository : ILogRepository
{
  private readonly IMongoCollection<LogEntity> _logs;
  private readonly JsonSerializerOptions _serializerOptions = new();

  public LogRepository(IMongoDatabase database, IServiceProvider serviceProvider)
  {
    _logs = database.GetCollection<LogEntity>("logs");
    _serializerOptions.Converters.AddRange(serviceProvider.GetJsonConverters());
  }

  public async Task SaveAsync(Log log, CancellationToken cancellationToken)
  {
    LogEntity entity = new(log, _serializerOptions);

    await _logs.InsertOneAsync(entity, new InsertOneOptions(), cancellationToken);
  }
}
