using Logitar.Identity.Contracts;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using SkillCraft.Application.Accounts;

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

  public async Task<User> CreateAsync(EmailPayload email, CancellationToken cancellationToken)
  {
    CreateUserPayload payload = new(email.Address)
    {
      Email = email
    };
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

  public async Task SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    RequestContext context = new(id.ToString(), cancellationToken);
    await _userClient.SignOutAsync(id, context);
  }

  public async Task<User> UpdateAsync(User user, EmailPayload email, CancellationToken cancellationToken)
  {
    if (user.Email == null || user.Email.Address != email.Address || user.Email.IsVerified != email.IsVerified)
    {
      UpdateUserPayload payload = new()
      {
        Email = new Modification<EmailPayload>(email)
      };
      RequestContext context = new(user.Id.ToString(), cancellationToken);
      return await _userClient.UpdateAsync(user.Id, payload, context) ?? throw new InvalidOperationException($"The user 'Id={user.Id}' could not be found.");
    }

    return user;
  }
}
