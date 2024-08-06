using Logitar;
using Logitar.Portal.Contracts.Errors;

namespace SkillCraft.Application.Storage;

public class NotEnoughAvailableStorageException : PaymentRequiredException
{
  private const string ErrorMessage = "There is not enough available storage.";

  public Guid UserId
  {
    get => (Guid)Data[nameof(UserId)]!;
    private set => Data[nameof(UserId)] = value;
  }
  public long AvailableBytes
  {
    get => (long)Data[nameof(AvailableBytes)]!;
    private set => Data[nameof(AvailableBytes)] = value;
  }
  public long RequiredBytes
  {
    get => (long)Data[nameof(RequiredBytes)]!;
    private set => Data[nameof(RequiredBytes)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.AddData(nameof(AvailableBytes), AvailableBytes.ToString());
      error.AddData(nameof(RequiredBytes), RequiredBytes.ToString());
      return error;
    }
  }

  public NotEnoughAvailableStorageException(Guid userId, long availableBytes, long requiredBytes)
    : base(BuildMessage(userId, availableBytes, requiredBytes))
  {
    UserId = userId;
    AvailableBytes = availableBytes;
    RequiredBytes = requiredBytes;
  }

  private static string BuildMessage(Guid userId, long availableBytes, long requiredBytes) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(UserId), userId)
    .AddData(nameof(AvailableBytes), availableBytes)
    .AddData(nameof(RequiredBytes), requiredBytes)
    .Build();
}
