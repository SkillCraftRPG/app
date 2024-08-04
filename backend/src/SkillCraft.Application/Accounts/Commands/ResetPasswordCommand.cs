using Logitar;
using Logitar.Portal.Contracts;
using MediatR;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Commands;

public record ResetPasswordCommand(ResetPasswordPayload Payload, IEnumerable<CustomAttribute> CustomAttributes) : Activity, IRequest<ResetPasswordResult>
{
  public override IActivity Anonymize()
  {
    ResetPasswordCommand command = this.DeepClone();
    if (Payload.Reset != null && command.Payload.Reset != null)
    {
      command.Payload.Reset = new(Payload.Reset.Token, Payload.Reset.Password.Mask());
    }
    return command;
  }
}
