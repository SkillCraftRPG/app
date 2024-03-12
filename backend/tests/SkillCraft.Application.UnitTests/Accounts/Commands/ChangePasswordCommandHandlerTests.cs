using Bogus;
using Logitar.Identity.Domain.Settings;
using Logitar.Portal.Contracts.Users;
using Moq;

namespace SkillCraft.Application.Accounts.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class ChangePasswordCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();
  private readonly UserSettings _userSettings = new()
  {
    UniqueName = new UniqueNameSettings(),
    Password = new PasswordSettings(),
    RequireUniqueEmail = true
  };

  private readonly Mock<IRealmService> _realmService = new();
  private readonly Mock<IUserService> _userService = new();

  private readonly ChangePasswordCommandHandler _handler;

  private readonly User _user;

  public ChangePasswordCommandHandlerTests()
  {
    _handler = new(_realmService.Object, _userService.Object);

    _realmService.Setup(x => x.GetUserSettingsAsync(_cancellationToken)).ReturnsAsync(_userSettings);

    _user = new(_faker.Person.UserName);
  }

  [Fact(DisplayName = "It should change the user password.")]
  public async Task It_should_change_the_user_password()
  {
    Contracts.Accounts.ChangePasswordPayload payload = new(current: "P@s$W0rD", @new: "Test123!");
    ChangePasswordCommand command = new(_user, payload);
    _userService.Setup(x => x.ChangePasswordAsync(_user, payload, _cancellationToken)).ReturnsAsync(_user);

    User user = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_user, user);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    Contracts.Accounts.ChangePasswordPayload payload = new(current: "P@s$W0rD", @new: "AAaa!!11");
    ChangePasswordCommand command = new(_user, payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.All(exception.Errors, e => Assert.Equal("New", e.PropertyName));
  }
}
