using FluentValidation;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Users;
using MediatR;
using SkillCraft.Application.Accounts.Validators;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Commands;

internal class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, User>
{
  private readonly IRealmService _realmService;
  private readonly IUserService _userService;

  public ChangePasswordCommandHandler(IRealmService realmService, IUserService userService)
  {
    _realmService = realmService;
    _userService = userService;
  }

  public async Task<User> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
  {
    Realm realm = await _realmService.FindAsync(cancellationToken);

    ChangeAccountPasswordPayload payload = command.Payload;
    new ChangePasswordValidator(realm.PasswordSettings).ValidateAndThrow(payload);

    User user = command.GetUser();

    return await _userService.ChangePasswordAsync(user, payload, cancellationToken);
  }
}
