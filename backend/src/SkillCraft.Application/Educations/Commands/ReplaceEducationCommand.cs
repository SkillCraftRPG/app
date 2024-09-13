using FluentValidation;
using MediatR;
using SkillCraft.Application.Educations.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Educations;
using SkillCraft.Domain;
using SkillCraft.Domain.Educations;

namespace SkillCraft.Application.Educations.Commands;

public record ReplaceEducationCommand(Guid Id, ReplaceEducationPayload Payload, long? Version) : Activity, IRequest<EducationModel?>;

internal class ReplaceEducationCommandHandler : IRequestHandler<ReplaceEducationCommand, EducationModel?>
{
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;
  private readonly IEducationQuerier _educationQuerier;
  private readonly IEducationRepository _educationRepository;

  public ReplaceEducationCommandHandler(
    IEducationQuerier educationQuerier,
    IEducationRepository educationRepository,
    IPermissionService permissionService,
    ISender sender)
  {
    _permissionService = permissionService;
    _sender = sender;
    _educationQuerier = educationQuerier;
    _educationRepository = educationRepository;
  }

  public async Task<EducationModel?> Handle(ReplaceEducationCommand command, CancellationToken cancellationToken)
  {
    ReplaceEducationPayload payload = command.Payload;
    new ReplaceEducationValidator().ValidateAndThrow(payload);

    EducationId id = new(command.Id);
    Education? education = await _educationRepository.LoadAsync(id, cancellationToken);
    if (education == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, EntityMetadata.From(education), cancellationToken);

    Education? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _educationRepository.LoadAsync(id, command.Version.Value, cancellationToken);
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
    education.Update(command.GetUserId());

    await _sender.Send(new SaveEducationCommand(education), cancellationToken);

    return await _educationQuerier.ReadAsync(education, cancellationToken);
  }
}
