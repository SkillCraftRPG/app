using FluentValidation;
using MediatR;
using SkillCraft.Application.Educations.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Educations;
using SkillCraft.Domain;
using SkillCraft.Domain.Educations;

namespace SkillCraft.Application.Educations.Commands;

public record UpdateEducationCommand(Guid Id, UpdateEducationPayload Payload) : Activity, IRequest<EducationModel?>;

internal class UpdateEducationCommandHandler : IRequestHandler<UpdateEducationCommand, EducationModel?>
{
  private readonly IEducationQuerier _educationQuerier;
  private readonly IEducationRepository _educationRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public UpdateEducationCommandHandler(IEducationQuerier educationQuerier, IEducationRepository educationRepository, IPermissionService permissionService, ISender sender)
  {
    _educationQuerier = educationQuerier;
    _educationRepository = educationRepository;
    _permissionService = permissionService;
    _sender = sender;
  }

  public async Task<EducationModel?> Handle(UpdateEducationCommand command, CancellationToken cancellationToken)
  {
    UpdateEducationPayload payload = command.Payload;
    new UpdateEducationValidator().ValidateAndThrow(payload);

    EducationId id = new(command.Id);
    Education? education = await _educationRepository.LoadAsync(id, cancellationToken);
    if (education == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, EntityMetadata.From(education), cancellationToken);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      education.Name = new Name(payload.Name);
    }
    if (payload.Description != null)
    {
      education.Description = Description.TryCreate(payload.Description.Value);
    }

    if (payload.Skill != null)
    {
      education.Skill = payload.Skill.Value;
    }
    if (payload.WealthMultiplier != null)
    {
      education.WealthMultiplier = payload.WealthMultiplier.Value;
    }
    education.Update(command.GetUserId());

    await _sender.Send(new SaveEducationCommand(education), cancellationToken);

    return await _educationQuerier.ReadAsync(education, cancellationToken);
  }
}
