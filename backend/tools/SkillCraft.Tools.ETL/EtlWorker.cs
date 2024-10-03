using Logitar.Net.Http;
using MediatR;
using SkillCraft.Tools.ETL.Commands;

namespace SkillCraft.Tools.ETL;

public class EtlWorker : BackgroundService
{
  private const string GenericErrorMessage = "An unhanded exception occurred.";

  private readonly IHostApplicationLifetime _hostApplicationLifetime;
  private readonly ILogger<EtlWorker> _logger;
  private readonly IServiceProvider _serviceProvider;

  private LogLevel _result = LogLevel.Information; // NOTE(fpion): "Information" means success.

  public EtlWorker(IHostApplicationLifetime hostApplicationLifetime, ILogger<EtlWorker> logger, IServiceProvider serviceProvider)
  {
    _hostApplicationLifetime = hostApplicationLifetime;
    _logger = logger;
    _serviceProvider = serviceProvider;
  }

  protected override async Task ExecuteAsync(CancellationToken cancellationToken)
  {
    Stopwatch chrono = Stopwatch.StartNew();
    _logger.LogInformation("Worker executing at {Timestamp}.", DateTimeOffset.Now);

    using IServiceScope scope = _serviceProvider.CreateScope();
    IPublisher publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

    await Task.Delay(5000, cancellationToken);

    try
    {
      await publisher.Publish(new ImportWorlds(), cancellationToken);
      await publisher.Publish(new ImportParties(), cancellationToken);
      await publisher.Publish(new ImportAspects(), cancellationToken);
      await publisher.Publish(new ImportCastes(), cancellationToken);
      await publisher.Publish(new ImportCustomizations(), cancellationToken);
      await publisher.Publish(new ImportEducations(), cancellationToken);
      await publisher.Publish(new ImportPersonalities(), cancellationToken);
      await publisher.Publish(new ImportLanguages(), cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, GenericErrorMessage);
      _result = LogLevel.Error;

      if (exception is HttpFailureException<JsonApiResult> jsonException)
      {
        _logger.LogError("{JsonContent}", jsonException.Result.JsonContent);
      }

      Environment.ExitCode = exception.HResult;
    }
    finally
    {
      chrono.Stop();

      long seconds = chrono.ElapsedMilliseconds / 1000;
      string secondText = seconds <= 1 ? "second" : "seconds";
      switch (_result)
      {
        case LogLevel.Error:
          _logger.LogError("ETL failed after {Elapsed}ms ({Seconds} {SecondText}).", chrono.ElapsedMilliseconds, seconds, secondText);
          break;
        case LogLevel.Warning:
          _logger.LogWarning("ETL completed with warnings in {Elapsed}ms ({Seconds} {SecondText}).", chrono.ElapsedMilliseconds, seconds, secondText);
          break;
        default:
          _logger.LogInformation("ETL succeeded in {Elapsed}ms ({Seconds} {SecondText}).", chrono.ElapsedMilliseconds, seconds, secondText);
          break;
      }

      _hostApplicationLifetime.StopApplication();
    }
  }
}
