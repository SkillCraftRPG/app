namespace SkillCraft.Application.Accounts.Constants;

internal static class TokenTypes
{
  public const string Authentication = "auth+jwt";
  public const string Profile = "profile+jwt";
  public const string PasswordRecovery = "reset_password+jwt";
}
