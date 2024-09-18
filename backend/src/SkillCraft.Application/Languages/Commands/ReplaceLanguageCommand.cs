using FluentValidation;
using MediatR;
using SkillCraft.Application.Languages.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Languages;
using SkillCraft.Domain;
using SkillCraft.Domain.Languages;

namespace SkillCraft.Application.Languages.Commands;

public record ReplaceLanguageCommand(Guid Id, ReplaceLanguagePayload Payload, long? Version) : Activity, IRequest<LanguageModel?>;

internal class ReplaceLanguageCommandHandler : IRequestHandler<ReplaceLanguageCommand, LanguageModel?>
{
  private readonly ILanguageQuerier _languageQuerier;
  private readonly ILanguageRepository _languageRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public ReplaceLanguageCommandHandler(
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

  public async Task<LanguageModel?> Handle(ReplaceLanguageCommand command, CancellationToken cancellationToken)
  {
    ReplaceLanguagePayload payload = command.Payload;
    new ReplaceLanguageValidator().ValidateAndThrow(payload);

    LanguageId id = new(command.Id);
    Language? language = await _languageRepository.LoadAsync(id, cancellationToken);
    if (language == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, language.GetMetadata(), cancellationToken);

    Language? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _languageRepository.LoadAsync(id, command.Version.Value, cancellationToken);
    }
    reference ??= language;

    Name name = new(payload.Name);
    if (name != reference.Name)
    {
      language.Name = name;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (description != reference.Description)
    {
      language.Description = description;
    }

    Script? script = Script.TryCreate(payload.Script);
    if (script != reference.Script)
    {
      language.Script = script;
    }
    TypicalSpeakers? typicalSpeakers = TypicalSpeakers.TryCreate(payload.TypicalSpeakers);
    if (typicalSpeakers != reference.TypicalSpeakers)
    {
      language.TypicalSpeakers = typicalSpeakers;
    }

    language.Update(command.GetUserId());
    await _sender.Send(new SaveLanguageCommand(language), cancellationToken);

    return await _languageQuerier.ReadAsync(language, cancellationToken);
  }
}
