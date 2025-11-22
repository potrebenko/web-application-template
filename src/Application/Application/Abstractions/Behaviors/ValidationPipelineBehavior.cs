using FluentValidation;
using FluentValidation.Results;

namespace Application.Abstractions.Behaviors;

internal sealed class ValidationPipelineBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators, 
    ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : class
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ValidationFailure[] validationFailures = await ValidateAsync(request, validators);

        if (validationFailures.Length == 0)
        {
            return await next();
        }
        
        logger.LogError("Validation failed: {@ValidationFailures}", validationFailures);

        return default;
    }

    private static async Task<ValidationFailure[]> ValidateAsync(TRequest request,
        IEnumerable<IValidator<TRequest>> validators)
    {
        if (!validators.Any())
        {
            return [];
        }
        
        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context)));
        var failures = validationResults
            .Where(validationResult => validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)
            .ToArray();
        return failures;
    }
}