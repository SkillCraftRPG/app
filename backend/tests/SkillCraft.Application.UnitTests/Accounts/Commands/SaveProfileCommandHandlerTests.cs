using Bogus;
using Logitar.Portal.Contracts.Users;
using Moq;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveProfileCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IUserService> _userService = new();

  private readonly SaveProfileCommandHandler _handler;

  private readonly User _user;

  public SaveProfileCommandHandlerTests()
  {
    _handler = new(_userService.Object);

    _user = new(_faker.Person.UserName);
  }

  [Fact(DisplayName = "It should save the user profile.")]
  public async Task It_should_save_the_user_profile()
  {
    SaveProfilePayload payload = new(_faker.Person.FirstName, _faker.Person.LastName, "fr-CA", "America/Montreal");
    SaveProfileCommand command = new(_user, payload);
    _userService.Setup(x => x.SaveProfileAsync(_user, payload, _cancellationToken)).ReturnsAsync(_user);

    User user = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_user, user);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    SaveProfilePayload payload = new(_faker.Person.FirstName, _faker.Person.LastName, "fr-CA", "America/Montreal")
    {
      MultiFactorAuthenticationMode = MultiFactorAuthenticationMode.Phone,
      Phone = null
    };
    SaveProfileCommand command = new(_user, payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal("SaveProfileValidator", Assert.Single(exception.Errors).ErrorCode);
  }
}
