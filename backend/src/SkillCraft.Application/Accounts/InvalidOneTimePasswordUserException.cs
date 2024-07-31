using Logitar;
using Logitar.Portal.Contracts.Errors;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Users;
using SkillCraft.Contracts.Errors;

namespace SkillCraft.Application.Accounts;

public class InvalidOneTimePasswordUserException : BadRequestException
{
  private const string ErrorMessage = "The specified user did not match the expected One-Time Passord (OTP) user.";

  public Guid OneTimePasswordId
  {
    get => (Guid)Data[nameof(OneTimePasswordId)]!;
    private set => Data[nameof(OneTimePasswordId)] = value;
  }
  public Guid? ExpectedUserId
  {
    get => (Guid?)Data[nameof(ExpectedUserId)];
    private set => Data[nameof(ExpectedUserId)] = value;
  }
  public Guid? ActualUserId
  {
    get => (Guid?)Data[nameof(ActualUserId)];
    private set => Data[nameof(ActualUserId)] = value;
  }

  public override Error Error => new InvalidCredentialsError();

  public InvalidOneTimePasswordUserException(OneTimePassword oneTimePassword, User? user) : base(BuildMessage(oneTimePassword, user))
  {
    OneTimePasswordId = oneTimePassword.Id;
    ExpectedUserId = oneTimePassword.TryGetUserId();
    ActualUserId = user?.Id;
  }

  private static string BuildMessage(OneTimePassword oneTimePassword, User? user) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(OneTimePasswordId), oneTimePassword.Id)
    .AddData(nameof(ExpectedUserId), oneTimePassword.TryGetUserId(), "<null>")
    .AddData(nameof(ActualUserId), user?.Id, "<null>")
    .Build();
}
