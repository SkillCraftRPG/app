using FluentValidation;
using FluentValidation.Results;
using Logitar;
using SkillCraft.Contracts.Errors;

namespace SkillCraft.Models.Errors;

public record ValidationError : Error
{
  public List<ValidationFailure> Errors { get; set; }

  public ValidationError() : this(string.Empty, string.Empty)
  {
  }

  public ValidationError(string code, string message) : base(code, message)
  {
    Errors = [];
  }

  public ValidationError(string code, string message, IEnumerable<ValidationFailure> errors) : this(code, message)
  {
    Errors.AddRange(errors);
  }

  public ValidationError(ValidationException exception) : this(exception.GetErrorCode(), "Validation failed.", exception.Errors)
  {
  }
}
