using Logitar.EventSourcing;
using Logitar.EventSourcing.Infrastructure;
using MediatR;

namespace SkillCraft.Infrastructure;

internal class EventBus : IEventBus
{
  //private readonly ILoggingService _loggingService;
  private readonly IPublisher _publisher;

  public EventBus(/*ILoggingService loggingService,*/ IPublisher publisher)
  {
    //_loggingService = loggingService;
    _publisher = publisher;
  }

  public async Task PublishAsync(DomainEvent change, CancellationToken cancellationToken)
  {
    //_loggingService.Report(change); // ISSUE: https://github.com/SkillCraftRPG/app/issues/3

    await _publisher.Publish(change, cancellationToken);
  }
}
