using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Models.Account;

public record GetTokenPayload : SignInPayload
{
  public string? RefreshToken { get; set; }
}
