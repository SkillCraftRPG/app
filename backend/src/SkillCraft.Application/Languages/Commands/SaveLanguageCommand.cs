using MediatR;
using SkillCraft.Application.Storages;
using SkillCraft.Domain.Languages;

namespace SkillCraft.Application.Languages.Commands;

internal record SaveLanguageCommand(Language Language) : IRequest;

internal class SaveLanguageCommandHandler : IRequestHandler<SaveLanguageCommand>
{
  private readonly ILanguageRepository _languageRepository;
  private readonly IStorageService _storageService;

  public SaveLanguageCommandHandler(ILanguageRepository languageRepository, IStorageService storageService)
  {
    _languageRepository = languageRepository;
    _storageService = storageService;
  }

  public async Task Handle(SaveLanguageCommand command, CancellationToken cancellationToken)
  {
    Language language = command.Language;

    EntityMetadata entity = language.GetMetadata();
    await _storageService.EnsureAvailableAsync(entity, cancellationToken);

    await _languageRepository.SaveAsync(language, cancellationToken);

    await _storageService.UpdateAsync(entity, cancellationToken);
  }
}
