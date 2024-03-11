using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Portal.Contracts.Users;
using MediatR;
using SkillCraft.Application.Accounts.Validators;

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
    IUserSettings userSettings = await _realmService.GetUserSettingsAsync(cancellationToken);

    Contracts.Accounts.ChangePasswordPayload payload = command.Payload;
    new ChangePasswordValidator(userSettings.Password).ValidateAndThrow(payload);

    return await _userService.ChangePasswordAsync(command.User, payload, cancellationToken);
  }
}
