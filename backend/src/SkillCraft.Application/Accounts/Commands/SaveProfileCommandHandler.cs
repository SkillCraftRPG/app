using FluentValidation;
using Logitar.Portal.Contracts.Users;
using MediatR;
using SkillCraft.Application.Accounts.Validators;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Commands;

internal class SaveProfileCommandHandler : IRequestHandler<SaveProfileCommand, User>
{
  private readonly IUserService _userService;

  public SaveProfileCommandHandler(IUserService userService)
  {
    _userService = userService;
  }

  public async Task<User> Handle(SaveProfileCommand command, CancellationToken cancellationToken)
  {
    SaveProfilePayload payload = command.Payload;
    new SaveProfileValidator().ValidateAndThrow(payload);

    User user = command.GetUser();

    return await _userService.SaveProfileAsync(user, payload, cancellationToken);
  }
}
