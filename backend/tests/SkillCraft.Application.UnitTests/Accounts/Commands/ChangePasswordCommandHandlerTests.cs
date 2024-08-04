using Bogus;
using FluentValidation.Results;
using Logitar.Portal.Contracts.Users;
using Moq;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class ChangePasswordCommandHandlerTests
{
  private const string PasswordString = "P@s$W0rD";

  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IRealmService> _realmService = new();
  private readonly Mock<IUserService> _userService = new();

  private readonly ChangePasswordCommandHandler _handler;

  private readonly RealmMock _realm = new();

  public ChangePasswordCommandHandlerTests()
  {
    _realmService.Setup(x => x.FindAsync(_cancellationToken)).ReturnsAsync(_realm);

    _handler = new(_realmService.Object, _userService.Object);
  }

  [Fact(DisplayName = "It should save the user profile.")]
  public async Task It_should_save_the_user_profile()
  {
    User user = new(_faker.Person.UserName);
    ChangeAccountPasswordPayload payload = new("Test123!", PasswordString);
    _userService.Setup(x => x.ChangePasswordAsync(user, payload, _cancellationToken)).ReturnsAsync(user);

    ChangePasswordCommand command = new(payload);
    command.Contextualize(new ActivityContextMock(user));
    User result = await _handler.Handle(command, _cancellationToken);

    Assert.Same(result, user);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_it_not_valid()
  {
    ChangeAccountPasswordPayload payload = new(string.Empty, PasswordString);
    ChangePasswordCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal("Current", error.PropertyName);
  }
}
