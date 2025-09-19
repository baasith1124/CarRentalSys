using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

        public ValidationBehavior(
            IEnumerable<IValidator<TRequest>> validators,
            ILogger<ValidationBehavior<TRequest, TResponse>> logger)
        {
            _validators = validators;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                _logger.LogDebug("Validating request of type {RequestType}", typeof(TRequest).Name);

                var context = new ValidationContext<TRequest>(request);
                
                // Execute all validators in parallel for better performance
                var validationResults = await Task.WhenAll(
                    _validators.Select(async validator =>
                    {
                        try
                        {
                            return await validator.ValidateAsync(context, cancellationToken);
                        }
                        catch (System.Exception ex)
                        {
                            _logger.LogError(ex, "Error occurred during validation with validator {ValidatorType}", validator.GetType().Name);
                            throw;
                        }
                    }));

                var failures = validationResults
                    .SelectMany(r => r.Errors)
                    .Where(f => f != null)
                    .ToList();

                if (failures.Count != 0)
                {
                    _logger.LogWarning("Validation failed for request of type {RequestType} with {FailureCount} failures", 
                        typeof(TRequest).Name, failures.Count);
                    
                    // Log validation failures for debugging
                    foreach (var failure in failures)
                    {
                        _logger.LogDebug("Validation failure: {PropertyName} - {ErrorMessage}", 
                            failure.PropertyName, failure.ErrorMessage);
                    }

                    throw new ValidationException(failures);
                }

                _logger.LogDebug("Validation passed for request of type {RequestType}", typeof(TRequest).Name);
            }

            return await next();
        }
    }
}
