using FluentValidation;
using MediatR;
using SkillCraft.Application.Languages.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Languages;
using SkillCraft.Domain;
using SkillCraft.Domain.Languages;

namespace SkillCraft.Application.Languages.Commands;

public record UpdateLanguageCommand(Guid Id, UpdateLanguagePayload Payload) : Activity, IRequest<LanguageModel?>;

internal class UpdateLanguageCommandHandler : IRequestHandler<UpdateLanguageCommand, LanguageModel?>
{
  private readonly ILanguageQuerier _languageQuerier;
  private readonly ILanguageRepository _languageRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public UpdateLanguageCommandHandler(
    ILanguageQuerier languageQuerier,
    ILanguageRepository languageRepository,
    IPermissionService permissionService,
    ISender sender)
  {
    _languageQuerier = languageQuerier;
    _languageRepository = languageRepository;
    _permissionService = permissionService;
    _sender = sender;
  }

  public async Task<LanguageModel?> Handle(UpdateLanguageCommand command, CancellationToken cancellationToken)
  {
    UpdateLanguagePayload payload = command.Payload;
    new UpdateLanguageValidator().ValidateAndThrow(payload);

    LanguageId id = new(command.Id);
    Language? language = await _languageRepository.LoadAsync(id, cancellationToken);
    if (language == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, language.GetMetadata(), cancellationToken);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      language.Name = new Name(payload.Name);
    }
    if (payload.Description != null)
    {
      language.Description = Description.TryCreate(payload.Description.Value);
    }

    if (payload.Script != null)
    {
      language.Script = Script.TryCreate(payload.Script.Value);
    }
    if (payload.TypicalSpeakers != null)
    {
      language.TypicalSpeakers = TypicalSpeakers.TryCreate(payload.TypicalSpeakers.Value);
    }

    language.Update(command.GetUserId());
    await _sender.Send(new SaveLanguageCommand(language), cancellationToken);

    return await _languageQuerier.ReadAsync(language, cancellationToken);
  }
}
