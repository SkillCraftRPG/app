using FluentValidation;
using MediatR;
using SkillCraft.Application.Languages.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Languages;
using SkillCraft.Domain;
using SkillCraft.Domain.Languages;

namespace SkillCraft.Application.Languages.Commands;

public record CreateOrReplaceLanguageResult(LanguageModel? Language = null, bool Created = false);

public record CreateOrReplaceLanguageCommand(Guid? Id, CreateOrReplaceLanguagePayload Payload, long? Version) : Activity, IRequest<CreateOrReplaceLanguageResult>;

internal class CreateOrReplaceLanguageCommandHandler : IRequestHandler<CreateOrReplaceLanguageCommand, CreateOrReplaceLanguageResult>
{
  private readonly ILanguageQuerier _languageQuerier;
  private readonly ILanguageRepository _languageRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public CreateOrReplaceLanguageCommandHandler(
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

  public async Task<CreateOrReplaceLanguageResult> Handle(CreateOrReplaceLanguageCommand command, CancellationToken cancellationToken)
  {
    new CreateOrReplaceLanguageValidator().ValidateAndThrow(command.Payload);

    Language? language = await FindAsync(command, cancellationToken);
    bool created = false;
    if (language == null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceLanguageResult();
      }

      language = await CreateAsync(command, cancellationToken);
      created = true;
    }
    else
    {
      await ReplaceAsync(command, language, cancellationToken);
    }

    await _sender.Send(new SaveLanguageCommand(language), cancellationToken);

    LanguageModel model = await _languageQuerier.ReadAsync(language, cancellationToken);
    return new CreateOrReplaceLanguageResult(model, created);
  }

  private async Task<Language?> FindAsync(CreateOrReplaceLanguageCommand command, CancellationToken cancellationToken)
  {
    if (!command.Id.HasValue)
    {
      return null;
    }

    LanguageId id = new(command.GetWorldId(), command.Id.Value);
    return await _languageRepository.LoadAsync(id, cancellationToken);
  }

  private async Task<Language> CreateAsync(CreateOrReplaceLanguageCommand command, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanCreateAsync(command, EntityType.Language, cancellationToken);

    CreateOrReplaceLanguagePayload payload = command.Payload;
    UserId userId = command.GetUserId();

    Language language = new(command.GetWorldId(), new Name(payload.Name), userId, command.Id)
    {
      Description = Description.TryCreate(payload.Description),
      Script = Script.TryCreate(payload.Script),
      TypicalSpeakers = TypicalSpeakers.TryCreate(payload.TypicalSpeakers)
    };
    language.Update(userId);

    return language;
  }

  private async Task ReplaceAsync(CreateOrReplaceLanguageCommand command, Language language, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanUpdateAsync(command, language.GetMetadata(), cancellationToken);

    CreateOrReplaceLanguagePayload payload = command.Payload;
    UserId userId = command.GetUserId();

    Language? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _languageRepository.LoadAsync(language.Id, command.Version.Value, cancellationToken);
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

    language.Update(userId);
  }
}
