﻿using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Lineages;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Application.Characters;

internal class InvalidExtraAttributesException : BadRequestException
{
  private const string ErrorMessage = "The specified extra attributes did not match the lineages expected extra attribute count.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid LineageId
  {
    get => (Guid)Data[nameof(LineageId)]!;
    private set => Data[nameof(LineageId)] = value;
  }
  public IEnumerable<Attribute> ExtraAttributes
  {
    get => (IEnumerable<Attribute>)Data[nameof(ExtraAttributes)]!;
    private set => Data[nameof(ExtraAttributes)] = value;
  }
  public int ExpectedCount
  {
    get => (int)Data[nameof(ExpectedCount)]!;
    private set => Data[nameof(ExpectedCount)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      PropertyError error = new(this.GetErrorCode(), ErrorMessage, ExtraAttributes, PropertyName);
      error.AddData(nameof(ExpectedCount), ExpectedCount.ToString());
      return error;
    }
  }

  public InvalidExtraAttributesException(Lineage lineage, IEnumerable<Attribute> extraAttributes, int expectedCount, string propertyName)
    : base(BuildMessage(lineage, extraAttributes, expectedCount, propertyName))
  {
    WorldId = lineage.WorldId.ToGuid();
    LineageId = lineage.EntityId;
    ExtraAttributes = extraAttributes;
    ExpectedCount = expectedCount;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Lineage lineage, IEnumerable<Attribute> extraAttributes, int expectedCount, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append(nameof(WorldId)).Append(": ").Append(lineage.WorldId.ToGuid()).AppendLine();
    message.Append(nameof(LineageId)).Append(": ").Append(lineage.Id.EntityId).AppendLine();
    message.Append(nameof(ExpectedCount)).Append(": ").Append(expectedCount).AppendLine();
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName);
    message.Append(nameof(ExtraAttributes)).Append(':').AppendLine();
    foreach (Attribute extraAttribute in extraAttributes)
    {
      message.Append(" - ").Append(extraAttribute).AppendLine();
    }

    return message.ToString();
  }
}
