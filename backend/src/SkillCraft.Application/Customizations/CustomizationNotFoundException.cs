﻿using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Customizations;

namespace SkillCraft.Application.Customizations;

internal class CustomizationNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified customization could not be found.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid Id
  {
    get => (Guid)Data[nameof(Id)]!;
    private set => Data[nameof(Id)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, Id, PropertyName);

  public CustomizationNotFoundException(CustomizationId customizationId, string propertyName)
    : base(BuildMessage(customizationId, propertyName))
  {
    WorldId = customizationId.WorldId.ToGuid();
    Id = customizationId.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(CustomizationId customizationId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), customizationId.WorldId.ToGuid())
    .AddData(nameof(Id), customizationId.EntityId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
