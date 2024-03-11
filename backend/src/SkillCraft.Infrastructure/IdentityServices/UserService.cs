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

  public async Task<User> ChangePasswordAsync(User user, Contracts.Accounts.ChangePasswordPayload payload, CancellationToken cancellationToken)
  {
    UpdateUserPayload updatePayload = new()
    {
      Password = new Logitar.Portal.Contracts.Users.ChangePasswordPayload(payload.New)
      {
        Current = payload.Current
      }
    };
    RequestContext context = new(user.Id.ToString(), cancellationToken);
    return await _userClient.UpdateAsync(user.Id, updatePayload, context)
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

  public async Task<User> ResetPasswordAsync(Guid id, string password, CancellationToken cancellationToken)
  {
    ResetUserPasswordPayload payload = new(password);
    RequestContext context = new(user: id.ToString(), cancellationToken);
    return await _userClient.ResetPasswordAsync(id, payload, context)
      ?? throw new InvalidOperationException($"The user 'Id={id}' password reset returned null.");
  }

  public async Task<User> SaveProfileAsync(User user, SaveProfilePayload profile, CancellationToken cancellationToken)
  {
    UpdateUserPayload payload = new()
    {
      Phone = new Modification<PhonePayload>(profile.Phone?.ToPhonePayload()),
      FirstName = new Modification<string>(profile.FirstName),
      LastName = new Modification<string>(profile.LastName),
      Birthdate = new Modification<DateTime?>(profile.Birthdate),
      Gender = new Modification<string>(profile.Gender),
      Locale = new Modification<string>(profile.Locale),
      TimeZone = new Modification<string>(profile.TimeZone)
    };

    if (profile is CompleteProfilePayload completedProfile)
    {
      if (completedProfile.Password != null)
      {
        payload.Password = new Logitar.Portal.Contracts.Users.ChangePasswordPayload(completedProfile.Password);
      }
      payload.CompleteProfile();
    }

    RequestContext context = new(user.Id.ToString(), cancellationToken);
    return await _userClient.UpdateAsync(user.Id, payload, context)
      ?? throw new InvalidOperationException($"The user 'Id={user.Id}' update returned null.");
  }

  public async Task<User> SignOutAsync(User user, CancellationToken cancellationToken)
  {
    RequestContext context = new(user.Id.ToString(), cancellationToken);
    return await _userClient.SignOutAsync(user.Id, context)
      ?? throw new InvalidOperationException($"The user 'Id={user.Id}' sign-out returned null.");
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
