using FluentValidation;
using MediatR;
using SkillCraft.Application.Educations.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Educations;
using SkillCraft.Domain;
using SkillCraft.Domain.Educations;

namespace SkillCraft.Application.Educations.Commands;

public record CreateOrReplaceEducationResult(EducationModel? Education = null, bool Created = false);

/// <exception cref="NotEnoughAvailableStorageException"></exception>
/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="ValidationException"></exception>
public record CreateOrReplaceEducationCommand(Guid? Id, CreateOrReplaceEducationPayload Payload, long? Version) : Activity, IRequest<CreateOrReplaceEducationResult>;

internal class CreateOrReplaceEducationCommandHandler : IRequestHandler<CreateOrReplaceEducationCommand, CreateOrReplaceEducationResult>
{
  private readonly IEducationQuerier _educationQuerier;
  private readonly IEducationRepository _educationRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public CreateOrReplaceEducationCommandHandler(
    IEducationQuerier educationQuerier,
    IEducationRepository educationRepository,
    IPermissionService permissionService,
    ISender sender)
  {
    _educationQuerier = educationQuerier;
    _educationRepository = educationRepository;
    _permissionService = permissionService;
    _sender = sender;
  }

  public async Task<CreateOrReplaceEducationResult> Handle(CreateOrReplaceEducationCommand command, CancellationToken cancellationToken)
  {
    new CreateOrReplaceEducationValidator().ValidateAndThrow(command.Payload);

    Education? education = await FindAsync(command, cancellationToken);
    bool created = false;
    if (education == null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceEducationResult();
      }

      education = await CreateAsync(command, cancellationToken);
      created = true;
    }
    else
    {
      await ReplaceAsync(command, education, cancellationToken);
    }

    await _sender.Send(new SaveEducationCommand(education), cancellationToken);

    EducationModel model = await _educationQuerier.ReadAsync(education, cancellationToken);
    return new CreateOrReplaceEducationResult(model, created);
  }

  private async Task<Education?> FindAsync(CreateOrReplaceEducationCommand command, CancellationToken cancellationToken)
  {
    if (!command.Id.HasValue)
    {
      return null;
    }

    EducationId id = new(command.GetWorldId(), command.Id.Value);
    return await _educationRepository.LoadAsync(id, cancellationToken);
  }

  private async Task<Education> CreateAsync(CreateOrReplaceEducationCommand command, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanCreateAsync(command, EntityType.Education, cancellationToken);

    CreateOrReplaceEducationPayload payload = command.Payload;
    UserId userId = command.GetUserId();

    Education education = new(command.GetWorldId(), new Name(payload.Name), userId, command.Id)
    {
      Description = Description.TryCreate(payload.Description),
      Skill = payload.Skill,
      WealthMultiplier = payload.WealthMultiplier
    };

    education.Update(userId);

    return education;
  }

  private async Task ReplaceAsync(CreateOrReplaceEducationCommand command, Education education, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanUpdateAsync(command, education.GetMetadata(), cancellationToken);

    CreateOrReplaceEducationPayload payload = command.Payload;
    UserId userId = command.GetUserId();

    Education? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _educationRepository.LoadAsync(education.Id, command.Version.Value, cancellationToken);
    }
    reference ??= education;

    Name name = new(payload.Name);
    if (name != reference.Name)
    {
      education.Name = name;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (description != reference.Description)
    {
      education.Description = description;
    }

    if (payload.Skill != reference.Skill)
    {
      education.Skill = payload.Skill;
    }
    if (payload.WealthMultiplier != reference.WealthMultiplier)
    {
      education.WealthMultiplier = payload.WealthMultiplier;
    }

    education.Update(userId);
  }
}
