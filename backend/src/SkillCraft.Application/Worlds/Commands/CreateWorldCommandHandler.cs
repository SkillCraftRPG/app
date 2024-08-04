using FluentValidation;
using Logitar.Identity.Domain.Shared;
using MediatR;
using SkillCraft.Application.Worlds.Validators;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds.Commands;

internal class CreateWorldCommandHandler : IRequestHandler<CreateWorldCommand, World>
{
  private readonly ISender _sender;
  private readonly IWorldQuerier _worldQuerier;

  public CreateWorldCommandHandler(ISender sender, IWorldQuerier worldQuerier)
  {
    _sender = sender;
    _worldQuerier = worldQuerier;
  }

  public async Task<World> Handle(CreateWorldCommand command, CancellationToken cancellationToken)
  {
    CreateWorldPayload payload = command.Payload;
    new CreateWorldValidator().ValidateAndThrow(payload);

    // TODO(fpion): 403
    // TODO(fpion): Storage Limitation

    WorldAggregate world = new(new SlugUnit(payload.UniqueSlug), command.ActorId)
    {
      DisplayName = DisplayNameUnit.TryCreate(payload.DisplayName),
      Description = DescriptionUnit.TryCreate(payload.Description)
    };
    world.Update(command.ActorId);

    await _sender.Send(new SaveWorldCommand(world), cancellationToken);

    // TODO(fpion): Storage Limitation

    return await _worldQuerier.ReadAsync(world, cancellationToken);
  }
}
