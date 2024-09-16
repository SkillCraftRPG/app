using FluentValidation;
using MediatR;
using SkillCraft.Application.Lineages.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain;
using SkillCraft.Domain.Lineages;

namespace SkillCraft.Application.Lineages.Commands;

public record UpdateLineageCommand(Guid Id, UpdateLineagePayload Payload) : Activity, IRequest<LineageModel?>;

internal class UpdateLineageCommandHandler : IRequestHandler<UpdateLineageCommand, LineageModel?>
{
  private readonly ILineageQuerier _lineageQuerier;
  private readonly ILineageRepository _lineageRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public UpdateLineageCommandHandler(
    ILineageQuerier lineageQuerier,
    ILineageRepository lineageRepository,
    IPermissionService permissionService,
    ISender sender)
  {
    _lineageQuerier = lineageQuerier;
    _lineageRepository = lineageRepository;
    _permissionService = permissionService;
    _sender = sender;
  }

  public async Task<LineageModel?> Handle(UpdateLineageCommand command, CancellationToken cancellationToken)
  {
    UpdateLineagePayload payload = command.Payload;
    new UpdateLineageValidator().ValidateAndThrow(payload);

    LineageId id = new(command.Id);
    Lineage? lineage = await _lineageRepository.LoadAsync(id, cancellationToken);
    if (lineage == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, lineage.GetMetadata(), cancellationToken);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      lineage.Name = new Name(payload.Name);
    }
    if (payload.Description != null)
    {
      lineage.Description = Description.TryCreate(payload.Description.Value);
    }

    lineage.Update(command.GetUserId());
    await _sender.Send(new SaveLineageCommand(lineage), cancellationToken);

    return await _lineageQuerier.ReadAsync(lineage, cancellationToken);
  }
}
