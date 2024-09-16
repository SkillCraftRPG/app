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
  private readonly ILineageRepository _lineageRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public CreateLineageCommandHandler(
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

  public async Task<LineageModel> Handle(CreateLineageCommand command, CancellationToken cancellationToken)
  {
    CreateLineagePayload payload = command.Payload;
    new CreateLineageValidator().ValidateAndThrow(payload);

    await _permissionService.EnsureCanCreateAsync(command, EntityType.Lineage, cancellationToken);

    Lineage? parent = null;
    if (payload.ParentId.HasValue)
    {
      LineageId parentId = new(payload.ParentId.Value);
      parent = await _lineageRepository.LoadAsync(parentId, cancellationToken)
        ?? throw new AggregateNotFoundException<Lineage>(parentId.AggregateId, nameof(payload.ParentId));
    }

    UserId userId = command.GetUserId();
    Lineage lineage = new(command.GetWorldId(), parent, new Name(payload.Name), userId)
    {
      Description = Description.TryCreate(payload.Description),
      Attributes = new(payload.Attributes)
    };

    SetTraits(lineage, payload);

    lineage.Update(userId);
    await _sender.Send(new SaveLineageCommand(lineage), cancellationToken);

    return await _lineageQuerier.ReadAsync(lineage, cancellationToken);
  }

  private static void SetTraits(Lineage lineage, CreateLineagePayload payload)
  {
    foreach (TraitPayload traitPayload in payload.Traits)
    {
      Trait trait = new(new Name(traitPayload.Name), Description.TryCreate(traitPayload.Description));
      if (traitPayload.Id.HasValue)
      {
        lineage.SetTrait(traitPayload.Id.Value, trait);
      }
      else
      {
        lineage.AddTrait(trait);
      }
    }
  }
}
