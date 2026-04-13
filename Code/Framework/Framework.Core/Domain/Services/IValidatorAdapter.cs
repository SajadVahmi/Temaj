namespace Framework.Core.Domain.Services;

public interface IValidatorAdapter 
{
    public ValidationResult Validate<T>(T model) where T : class;
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public Dictionary<string, string[]> Errors { get; set; } = new();

    public static ValidationResult Success() => new() { IsValid = true };

    public static ValidationResult Failure(Dictionary<string, string[]> errors) => new() { IsValid = false, Errors = errors };
}
