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

public record SaveEducationResult(EducationModel? Education = null, bool Created = false);

public record SaveEducationCommand(Guid? Id, SaveEducationPayload Payload, long? Version) : Activity, IRequest<SaveEducationResult>;

internal class SaveEducationCommandHandler : EducationCommandHandler, IRequestHandler<SaveEducationCommand, SaveEducationResult>
{
  private readonly IEducationQuerier _educationQuerier;
  private readonly IEducationRepository _educationRepository;
  private readonly IPermissionService _permissionService;

  public SaveEducationCommandHandler(
    IEducationQuerier educationQuerier,
    IEducationRepository educationRepository,
    IPermissionService permissionService,
    IStorageService storageService)
    : base(educationRepository, storageService)
  {
    _educationQuerier = educationQuerier;
    _educationRepository = educationRepository;
    _permissionService = permissionService;
  }

  public async Task<SaveEducationResult> Handle(SaveEducationCommand command, CancellationToken cancellationToken)
  {
    new SaveEducationValidator().ValidateAndThrow(command.Payload);

    Education? education = await FindAsync(command, cancellationToken);
    bool created = false;
    if (education == null)
    {
      if (command.Version.HasValue)
      {
        return new SaveEducationResult();
      }

      education = await CreateAsync(command, cancellationToken);
      created = true;
    }
    else
    {
      await ReplaceAsync(command, education, cancellationToken);
    }

    await SaveAsync(education, cancellationToken);

    EducationModel model = await _educationQuerier.ReadAsync(education, cancellationToken);
    return new SaveEducationResult(model, created);
  }

  private async Task<Education?> FindAsync(SaveEducationCommand command, CancellationToken cancellationToken)
  {
    if (!command.Id.HasValue)
    {
      return null;
    }

    EducationId id = new(command.GetWorldId(), command.Id.Value);
    return await _educationRepository.LoadAsync(id, cancellationToken);
  }

  private async Task<Education> CreateAsync(SaveEducationCommand command, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanCreateAsync(command, EntityType.Education, cancellationToken);

    SaveEducationPayload payload = command.Payload;
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

  private async Task ReplaceAsync(SaveEducationCommand command, Education education, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanUpdateAsync(command, education.GetMetadata(), cancellationToken);

    SaveEducationPayload payload = command.Payload;
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
