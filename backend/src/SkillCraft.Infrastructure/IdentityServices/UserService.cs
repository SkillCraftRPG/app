using Logitar.Identity.Contracts;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using SkillCraft.Application.Accounts;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Infrastructure.IdentityServices;

internal class UserService : IUserService
{
  private readonly IUserClient _userClient;

  public UserService(IUserClient userClient)
  {
    _userClient = userClient;
  }

  public async Task<User> AuthenticateAsync(User user, string password, CancellationToken cancellationToken)
  {
    AuthenticateUserPayload payload = new(user.UniqueName, password);
    RequestContext context = new(user.Id.ToString(), cancellationToken);
    return await _userClient.AuthenticateAsync(payload, context);
  }

  public async Task<User> CompleteProfileAsync(User user, ProfilePayload profile, CancellationToken cancellationToken)
  {
    UpdateUserPayload payload = new()
    {
      FirstName = new Modification<string>(profile.FirstName),
      LastName = new Modification<string>(profile.LastName),
      Locale = new Modification<string>(profile.Locale),
      TimeZone = new Modification<string>(profile.TimeZone)
    };
    if (profile.Password != null)
    {
      payload.Password = new ChangePasswordPayload(profile.Password);
    }
    payload.SetMultiFactorAuthenticationMode(profile.MultiFactorAuthenticationMode);
    if (profile.Phone != null)
    {
      payload.Phone = new Modification<PhonePayload>(profile.Phone);
    }
    if (profile.Birthdate.HasValue)
    {
      payload.Birthdate = new Modification<DateTime?>(profile.Birthdate.Value);
    }
    if (!string.IsNullOrWhiteSpace(profile.Gender))
    {
      payload.Gender = new Modification<string>(profile.Gender.Trim());
    }
    payload.CompleteProfile();
    RequestContext context = new(user.Id.ToString(), cancellationToken);
    return await _userClient.UpdateAsync(user.Id, payload, context)
      ?? throw new InvalidOperationException($"The user 'Id={user.Id}' update returned null.");
  }

  public async Task<User> CreateAsync(Email email, CancellationToken cancellationToken)
  {
    CreateUserPayload payload = new(email.Address)
    {
      Email = new EmailPayload(email.Address, email.IsVerified)
    };
    RequestContext context = new(cancellationToken);
    return await _userClient.CreateAsync(payload, context);
  }

  public async Task<User?> FindAsync(Guid id, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await _userClient.ReadAsync(id, uniqueName: null, identifier: null, context);
  }

  public async Task<User?> FindAsync(string uniqueName, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await _userClient.ReadAsync(id: null, uniqueName, identifier: null, context);
  }

  public async Task<User> UpdateEmailAsync(User user, Email email, CancellationToken cancellationToken)
  {
    UpdateUserPayload payload = new()
    {
      Email = new Modification<EmailPayload>(new EmailPayload(email.Address, email.IsVerified))
    };
    RequestContext context = new(user.Id.ToString(), cancellationToken);
    return await _userClient.UpdateAsync(user.Id, payload, context)
      ?? throw new InvalidOperationException($"The user 'Id={user.Id}' update returned null.");
  }
}
