using FluentValidation;
using MediatR;
using SkillCraft.Application.Lineages.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain;
using SkillCraft.Domain.Lineages;

namespace SkillCraft.Application.Lineages.Commands;

public record ReplaceLineageCommand(Guid Id, ReplaceLineagePayload Payload, long? Version) : Activity, IRequest<LineageModel?>;

internal class ReplaceLineageCommandHandler : IRequestHandler<ReplaceLineageCommand, LineageModel?>
{
  private readonly ILineageQuerier _lineageQuerier;
  private readonly ILineageRepository _lineageRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public ReplaceLineageCommandHandler(
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

  public async Task<LineageModel?> Handle(ReplaceLineageCommand command, CancellationToken cancellationToken)
  {
    ReplaceLineagePayload payload = command.Payload;
    new ReplaceLineageValidator().ValidateAndThrow(payload);

    LineageId id = new(command.Id);
    Lineage? lineage = await _lineageRepository.LoadAsync(id, cancellationToken);
    if (lineage == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, lineage.GetMetadata(), cancellationToken);

    Lineage? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _lineageRepository.LoadAsync(id, command.Version.Value, cancellationToken);
    }
    reference ??= lineage;

    Name name = new(payload.Name);
    if (name != reference.Name)
    {
      lineage.Name = name;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (description != reference.Description)
    {
      lineage.Description = description;
    }

    lineage.Update(command.GetUserId());
    await _sender.Send(new SaveLineageCommand(lineage), cancellationToken);

    return await _lineageQuerier.ReadAsync(lineage, cancellationToken);
  }
}
