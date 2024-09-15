using FluentValidation;
using MediatR;
using SkillCraft.Application.Languages.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Languages;
using SkillCraft.Domain;
using SkillCraft.Domain.Languages;

namespace SkillCraft.Application.Languages.Commands;

public record CreateLanguageCommand(CreateLanguagePayload Payload) : Activity, IRequest<LanguageModel>;

internal class CreateLanguageCommandHandler : IRequestHandler<CreateLanguageCommand, LanguageModel>
{
  private readonly ILanguageQuerier _languageQuerier;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public CreateLanguageCommandHandler(ILanguageQuerier languageQuerier, IPermissionService permissionService, ISender sender)
  {
    _languageQuerier = languageQuerier;
    _permissionService = permissionService;
    _sender = sender;
  }

  public async Task<LanguageModel> Handle(CreateLanguageCommand command, CancellationToken cancellationToken)
  {
    CreateLanguagePayload payload = command.Payload;
    new CreateLanguageValidator().ValidateAndThrow(payload);

    await _permissionService.EnsureCanCreateAsync(command, EntityType.Language, cancellationToken);

    UserId userId = command.GetUserId();
    Language language = new(command.GetWorldId(), new Name(payload.Name), userId)
    {
      Description = Description.TryCreate(payload.Description),
      Script = Script.TryCreate(payload.Script),
      TypicalSpeakers = TypicalSpeakers.TryCreate(payload.TypicalSpeakers)
    };

    language.Update(userId);
    await _sender.Send(new SaveLanguageCommand(language), cancellationToken);

    return await _languageQuerier.ReadAsync(language, cancellationToken);
  }
}
