using Logitar;
using Logitar.Portal.Contracts.Users;
using MediatR;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Commands;

public record ChangePasswordCommand(ChangeAccountPasswordPayload Payload) : Activity, IRequest<User>
{
  public override IActivity Anonymize()
  {
    ChangePasswordCommand command = this.DeepClone();
    command.Payload.Current = command.Payload.Current.Mask();
    command.Payload.New = command.Payload.New.Mask();
    return command;
  }
}
