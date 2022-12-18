using Codeflix.Catalog.Domain.Exceptions;
using System.Xml.Linq;

namespace Codeflix.Catalog.Domain.Validation;
public class DomainValidation
{
    public static void NotNull(object? target, string fieldName)
    {
        if(target == null) 
            throw new EntityValidationException($"{fieldName} should not be null");
    }

    public static void NotNullOrEmpty(string? target, string fieldName)
    {
        if (String.IsNullOrWhiteSpace(target))
            throw new EntityValidationException($"{fieldName} should not be null or empty");
    }

    public static void MinLength(string target, int minLength, string fieldName)
    {
        if (target.Length < minLength)
            throw new EntityValidationException($"{fieldName} should have at least {minLength} characters");
    }

    public static void MaxLength(string target, int maxLength, string fieldName)
    {
        if (target.Length > maxLength)
            throw new EntityValidationException($"{fieldName} should have no more than {maxLength} characters");
    }
}
