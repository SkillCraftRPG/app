using FluentValidation;
using MediatR;
using SkillCraft.Application.Lineages.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain;
using SkillCraft.Domain.Lineages;

namespace SkillCraft.Application.Lineages.Commands;

public record CreateLineageCommand(CreateLineagePayload Payload) : Activity, IRequest<LineageModel>;

internal class CreateLineageCommandHandler : IRequestHandler<CreateLineageCommand, LineageModel>
{
  private readonly ILineageQuerier _lineageQuerier;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public CreateLineageCommandHandler(ILineageQuerier lineageQuerier, IPermissionService permissionService, ISender sender)
  {
    _lineageQuerier = lineageQuerier;
    _permissionService = permissionService;
    _sender = sender;
  }

  public async Task<LineageModel> Handle(CreateLineageCommand command, CancellationToken cancellationToken)
  {
    CreateLineagePayload payload = command.Payload;
    new CreateLineageValidator().ValidateAndThrow(payload);

    await _permissionService.EnsureCanCreateAsync(command, EntityType.Lineage, cancellationToken);

    UserId userId = command.GetUserId();
    Lineage lineage = new(command.GetWorldId(), new Name(payload.Name), userId)
    {
      Description = Description.TryCreate(payload.Description)
    };

    lineage.Update(userId);
    await _sender.Send(new SaveLineageCommand(lineage), cancellationToken);

    return await _lineageQuerier.ReadAsync(lineage, cancellationToken);
  }
}
