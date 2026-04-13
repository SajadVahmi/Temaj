using FluentValidation;
using Framework.Core.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Framework.Infrastructure.Tools.Validators.FluentValidation
{
    public class FluentValidationAdapter(IServiceCollection serviceCollection) : IValidatorAdapter
    {

        public ValidationResult Validate<T>(T model) where T : class
        {
            var validator = serviceCollection
                .BuildServiceProvider()
                .GetService<IValidator<T>>();

            if (validator == null)
                throw new InvalidOperationException($"No validator registered for type {typeof(T).FullName}");

            var validationResult = validator.Validate(model);

            if (validationResult.IsValid)

                return ValidationResult.Success();

            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return ValidationResult.Failure(errors);

        }
    }
}
