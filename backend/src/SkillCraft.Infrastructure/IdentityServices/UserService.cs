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
    return await AuthenticateAsync(user.UniqueName, password, cancellationToken);
  }
  public async Task<User> AuthenticateAsync(string uniqueName, string password, CancellationToken cancellationToken)
  {
    AuthenticateUserPayload payload = new(uniqueName, password);
    RequestContext context = new(cancellationToken);
    return await _userClient.AuthenticateAsync(payload, context);
  }

  public async Task<User> ChangePasswordAsync(User user, ChangeAccountPasswordPayload input, CancellationToken cancellationToken)
  {
    UpdateUserPayload payload = new()
    {
      Password = input.ToChangePasswordPayload()
    };
    RequestContext context = new(user.Id.ToString(), cancellationToken);
    return await _userClient.UpdateAsync(user.Id, payload, context) ?? throw new InvalidOperationException($"The user 'Id={user.Id}' could not be found.");
  }

  public async Task<User> CompleteProfileAsync(User user, CompleteProfilePayload profile, PhonePayload? phone, CancellationToken cancellationToken)
  {
    UpdateUserPayload payload = profile.ToUpdateUserPayload();
    if (profile.Password != null)
    {
      payload.Password = new ChangePasswordPayload(profile.Password);
    }
    if (phone != null)
    {
      payload.Phone = new Modification<PhonePayload>(phone);
    }
    payload.CompleteProfile();
    RequestContext context = new(user.Id.ToString(), cancellationToken);
    return await _userClient.UpdateAsync(user.Id, payload, context) ?? throw new InvalidOperationException($"The user 'Id={user.Id}' could not be found.");
  }

  public async Task<User> CreateAsync(EmailPayload email, CancellationToken cancellationToken)
  {
    return await CreateAsync(email, identifier: null, cancellationToken);
  }
  public async Task<User> CreateAsync(EmailPayload email, CustomIdentifier? identifier, CancellationToken cancellationToken)
  {
    CreateUserPayload payload = new(email.Address)
    {
      Email = email
    };
    if (identifier != null)
    {
      payload.CustomIdentifiers.Add(identifier);
    }
    RequestContext context = new(cancellationToken);
    return await _userClient.CreateAsync(payload, context);
  }

  public async Task<User?> FindAsync(string emailAddress, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await _userClient.ReadAsync(id: null, emailAddress, identifier: null, context);
  }

  public async Task<User?> FindAsync(Guid id, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await _userClient.ReadAsync(id, uniqueName: null, identifier: null, context);
  }

  public async Task<User?> FindAsync(CustomIdentifier identifier, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await _userClient.ReadAsync(id: null, uniqueName: null, identifier, context);
  }

  public async Task<User> ResetPasswordAsync(User user, string password, CancellationToken cancellationToken)
  {
    ResetUserPasswordPayload payload = new(password);
    RequestContext context = new(user.Id.ToString(), cancellationToken);
    return await _userClient.ResetPasswordAsync(user.Id, payload, context) ?? throw new InvalidOperationException($"The user 'Id={user.Id}' could not be found.");
  }

  public async Task<User> SaveIdentifierAsync(User user, CustomIdentifier identifier, CancellationToken cancellationToken)
  {
    SaveUserIdentifierPayload payload = new(identifier.Value);
    RequestContext context = new(user.Id.ToString(), cancellationToken);
    return await _userClient.SaveIdentifierAsync(user.Id, identifier.Key, payload, context) ?? throw new InvalidOperationException($"The user 'Id={user.Id}' could not be found.");
  }

  public async Task<User> SaveProfileAsync(User user, SaveProfilePayload profile, CancellationToken cancellationToken)
  {
    UpdateUserPayload payload = profile.ToUpdateUserPayload();
    RequestContext context = new(user.Id.ToString(), cancellationToken);
    return await _userClient.UpdateAsync(user.Id, payload, context) ?? throw new InvalidOperationException($"The user 'Id={user.Id}' could not be found.");
  }

  public async Task SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    RequestContext context = new(id.ToString(), cancellationToken);
    await _userClient.SignOutAsync(id, context);
  }

  public async Task<User> UpdateAsync(User user, EmailPayload email, CancellationToken cancellationToken)
  {
    UpdateUserPayload payload = new()
    {
      Email = new Modification<EmailPayload>(email)
    };
    RequestContext context = new(user.Id.ToString(), cancellationToken);
    return await _userClient.UpdateAsync(user.Id, payload, context) ?? throw new InvalidOperationException($"The user 'Id={user.Id}' could not be found.");
  }

  public async Task<User> UpdateAsync(User user, PhonePayload phone, CancellationToken cancellationToken)
  {
    UpdateUserPayload payload = new()
    {
      Phone = new Modification<PhonePayload>(phone)
    };
    RequestContext context = new(user.Id.ToString(), cancellationToken);
    return await _userClient.UpdateAsync(user.Id, payload, context) ?? throw new InvalidOperationException($"The user 'Id={user.Id}' could not be found.");
  }
}
