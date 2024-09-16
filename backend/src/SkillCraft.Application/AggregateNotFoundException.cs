using Logitar;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Errors;
using SkillCraft.Contracts.Errors;

namespace SkillCraft.Application;

internal class AggregateNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified aggregate could not be found.";

  public string TypeName
  {
    get => (string)Data[nameof(TypeName)]!;
    private set => Data[nameof(TypeName)] = value;
  }
  public string Id
  {
    get => (string)Data[nameof(Id)]!;
    private set => Data[nameof(Id)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, new AggregateId(Id).ToGuid(), PropertyName);

  public AggregateNotFoundException(Type type, AggregateId id, string propertyName)
    : base(BuildMessage(type, id, propertyName))
  {
    if (!type.IsSubclassOf(typeof(AggregateRoot)))
    {
      throw new ArgumentException($"The type must be a subclass of the '{nameof(AggregateRoot)}' class.", nameof(type));
    }

    TypeName = type.GetNamespaceQualifiedName();
    Id = id.Value;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Type type, AggregateId id, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append(nameof(TypeName)).Append(": ").AppendLine(type.GetNamespaceQualifiedName());
    message.Append(nameof(Id)).Append(": ").Append(id).AppendLine();
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName);

    return message.ToString();
  }
}

internal class AggregateNotFoundException<T> : AggregateNotFoundException
{
  public AggregateNotFoundException(AggregateId id, string propertyName)
    : base(typeof(T), id, propertyName)
  {
  }
}
