using Logitar;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;
using SkillCraft.Domain.Talents;

namespace SkillCraft.Application.Talents;

internal class InvalidRequiredTalentTierException : BadRequestException
{
  private const string ErrorMessage = "The specified talent tier is higher than the requiring talent tier.";

  public Guid Id
  {
    get => (Guid)Data[nameof(Id)]!;
    private set => Data[nameof(Id)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, Id, PropertyName);

  public InvalidRequiredTalentTierException(Talent requiredTalent, string? propertyName)
    : base(BuildMessage(requiredTalent, propertyName))
  {
    Id = requiredTalent.Id.ToGuid();
    PropertyName = propertyName;
  }

  private static string BuildMessage(Talent requiredTalent, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Id), requiredTalent.Id.ToGuid())
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
