using FluentValidation.Results;

namespace Ordering.Application.Exceptions;

public class ValidatorException : ApplicationException
{
    public ValidatorException() : base("One or more validation failures have occured")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidatorException(IEnumerable<ValidationFailure> failures) : this()
    {
        Errors = failures.GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    public IDictionary<string, string[]> Errors { get; }
}