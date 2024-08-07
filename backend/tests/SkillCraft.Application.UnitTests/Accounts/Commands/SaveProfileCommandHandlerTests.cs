using Bogus;
using FluentValidation.Results;
using Logitar.Portal.Contracts.Users;
using Moq;
using SkillCraft.Application.Actors;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveProfileCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IActorService> _actorService = new();
  private readonly Mock<IUserService> _userService = new();

  private readonly SaveProfileCommandHandler _handler;

  public SaveProfileCommandHandlerTests()
  {
    _handler = new(_actorService.Object, _userService.Object);
  }

  [Fact(DisplayName = "It should save the user profile.")]
  public async Task It_should_save_the_user_profile()
  {
    User user = new(_faker.Person.UserName);
    SaveProfilePayload payload = new(_faker.Person.FirstName, _faker.Person.LastName, "fr", "America/Montreal")
    {
      MultiFactorAuthenticationMode = MultiFactorAuthenticationMode.Phone,
      Birthdate = _faker.Person.DateOfBirth,
      Gender = _faker.Person.Gender.ToString(),
      UserType = UserType.Gamemaster
    };
    _userService.Setup(x => x.SaveProfileAsync(user, payload, _cancellationToken)).ReturnsAsync(user);

    SaveProfileCommand command = new(payload);
    command.Contextualize(new ActivityContextMock(user));
    User result = await _handler.Handle(command, _cancellationToken);

    Assert.Same(result, user);

    _actorService.Verify(x => x.SaveAsync(user, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_it_not_valid()
  {
    SaveProfilePayload payload = new(_faker.Person.FirstName, _faker.Person.LastName, "fr", "America/Montreal")
    {
      Birthdate = DateTime.Now.AddYears(1)
    };
    SaveProfileCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("PastValidator", error.ErrorCode);
    Assert.Equal("Birthdate.Value", error.PropertyName);
  }
}
