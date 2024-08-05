using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Users;
using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Worlds.Validators;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds.Commands;

internal class CreateWorldCommandHandler : IRequestHandler<CreateWorldCommand, World>
{
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;
  private readonly IWorldQuerier _worldQuerier;

  public CreateWorldCommandHandler(IPermissionService permissionService, ISender sender, IWorldQuerier worldQuerier)
  {
    _permissionService = permissionService;
    _sender = sender;
    _worldQuerier = worldQuerier;
  }

  public async Task<World> Handle(CreateWorldCommand command, CancellationToken cancellationToken)
  {
    CreateWorldPayload payload = command.Payload;
    new CreateWorldValidator().ValidateAndThrow(payload);

    User user = command.GetUser();
    await _permissionService.EnsureCanCreateWorldAsync(user, cancellationToken);

    WorldAggregate world = new(new SlugUnit(payload.UniqueSlug), command.ActorId)
    {
      DisplayName = DisplayNameUnit.TryCreate(payload.DisplayName),
      Description = DescriptionUnit.TryCreate(payload.Description)
    };
    world.Update(command.ActorId);

    await _sender.Send(new SaveWorldCommand(user, world), cancellationToken);

    return await _worldQuerier.ReadAsync(world, cancellationToken);
  }
}
