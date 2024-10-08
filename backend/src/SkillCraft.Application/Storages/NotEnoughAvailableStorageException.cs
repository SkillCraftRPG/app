﻿using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Domain.Storages;

namespace SkillCraft.Application.Storages;

internal class NotEnoughAvailableStorageException : PaymentRequiredException
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

  public NotEnoughAvailableStorageException(Storage storage, long requiredBytes)
    : base(BuildMessage(storage, requiredBytes))
  {
    UserId = storage.UserId.ToGuid();
    AvailableBytes = storage.AvailableBytes;
    RequiredBytes = requiredBytes;
  }

  private static string BuildMessage(Storage storage, long requiredBytes) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(UserId), storage.UserId)
    .AddData(nameof(AvailableBytes), storage.AvailableBytes)
    .AddData(nameof(RequiredBytes), requiredBytes)
    .Build();
}
