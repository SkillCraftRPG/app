using MediatR;
using SkillCraft.Application.Storages;
using SkillCraft.Domain.Personalities;

namespace SkillCraft.Application.Personalities.Commands;

internal record SavePersonalityCommand(Personality Personality) : IRequest;

internal class SavePersonalityCommandHandler : IRequestHandler<SavePersonalityCommand>
{
  private readonly IPersonalityRepository _personalityRepository;
  private readonly IStorageService _storageService;

  public SavePersonalityCommandHandler(IPersonalityRepository personalityRepository, IStorageService storageService)
  {
    _personalityRepository = personalityRepository;
    _storageService = storageService;
  }

  public async Task Handle(SavePersonalityCommand command, CancellationToken cancellationToken)
  {
    Personality personality = command.Personality;

    EntityMetadata entity = personality.GetMetadata();
    await _storageService.EnsureAvailableAsync(entity, cancellationToken);

    await _personalityRepository.SaveAsync(personality, cancellationToken);

    await _storageService.UpdateAsync(entity, cancellationToken);
  }
}
