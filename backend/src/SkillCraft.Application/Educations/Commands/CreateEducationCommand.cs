using FluentValidation;
using MediatR;
using SkillCraft.Application.Educations.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Educations;
using SkillCraft.Domain;
using SkillCraft.Domain.Educations;

namespace SkillCraft.Application.Educations.Commands;

public record CreateEducationCommand(CreateEducationPayload Payload) : Activity, IRequest<EducationModel>;

internal class CreateEducationCommandHandler : IRequestHandler<CreateEducationCommand, EducationModel>
{
  private readonly IEducationQuerier _educationQuerier;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public CreateEducationCommandHandler(IEducationQuerier educationQuerier, IPermissionService permissionService, ISender sender)
  {
    _educationQuerier = educationQuerier;
    _permissionService = permissionService;
    _sender = sender;
  }

  public async Task<EducationModel> Handle(CreateEducationCommand command, CancellationToken cancellationToken)
  {
    CreateEducationPayload payload = command.Payload;
    new CreateEducationValidator().ValidateAndThrow(payload);

    await _permissionService.EnsureCanCreateAsync(command, EntityType.Education, cancellationToken);

    UserId userId = command.GetUserId();
    Education education = new(command.GetWorldId(), new Name(payload.Name), userId)
    {
      Description = Description.TryCreate(payload.Description),
      Skill = payload.Skill,
      WealthMultiplier = payload.WealthMultiplier
    };

    education.Update(userId);
    await _sender.Send(new SaveEducationCommand(education), cancellationToken);

    return await _educationQuerier.ReadAsync(education, cancellationToken);
  }
}
