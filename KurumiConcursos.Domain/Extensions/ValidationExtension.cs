using FluentValidation.Results;

namespace KurumiConcursos.Domain.Extensions;

public static class ValidationExtension
{
    public static Dictionary<string, string> ToValidationDictionary(
        this IEnumerable<ValidationFailure> errors)
    {
        var dictionary = new Dictionary<string, string>();
        foreach (var error in errors)
        {
            if (!dictionary.ContainsKey(error.PropertyName))
                dictionary.Add(error.PropertyName, error.ErrorMessage);
        }

        return dictionary;
    }
}
