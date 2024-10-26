using FluentValidation;
using Logitar.Portal.Contracts.Users;
using MediatR;
using SkillCraft.Application.Accounts.Validators;
using SkillCraft.Application.Actors;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Commands;

public record SaveProfileCommand(SaveProfilePayload Payload) : Activity, IRequest<User>;

internal class SaveProfileCommandHandler : IRequestHandler<SaveProfileCommand, User>
{
  private readonly IActorService _actorService;
  private readonly IUserService _userService;

  public SaveProfileCommandHandler(IActorService actorService, IUserService userService)
  {
    _actorService = actorService;
    _userService = userService;
  }

  public async Task<User> Handle(SaveProfileCommand command, CancellationToken cancellationToken)
  {
    SaveProfilePayload payload = command.Payload;
    new SaveProfileValidator().ValidateAndThrow(payload);

    User user = command.GetUser();
    user = await _userService.SaveProfileAsync(user, payload, cancellationToken);
    await _actorService.SaveAsync(user, cancellationToken);

    return user;
  }
}
