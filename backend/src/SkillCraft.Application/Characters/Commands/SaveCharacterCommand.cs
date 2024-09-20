using MediatR;
using SkillCraft.Application.Storages;
using SkillCraft.Domain.Characters;

namespace SkillCraft.Application.Characters.Commands;

internal record SaveCharacterCommand(Character Character) : IRequest;

internal class SaveCharacterCommandHandler : IRequestHandler<SaveCharacterCommand>
{
  private readonly ICharacterRepository _characterRepository;
  private readonly IStorageService _storageService;

  public SaveCharacterCommandHandler(ICharacterRepository characterRepository, IStorageService storageService)
  {
    _characterRepository = characterRepository;
    _storageService = storageService;
  }

  public async Task Handle(SaveCharacterCommand command, CancellationToken cancellationToken)
  {
    Character character = command.Character;

    EntityMetadata entity = character.GetMetadata();
    await _storageService.EnsureAvailableAsync(entity, cancellationToken);

    await _characterRepository.SaveAsync(character, cancellationToken);

    await _storageService.UpdateAsync(entity, cancellationToken);
  }
}
