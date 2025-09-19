using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Common.Validators
{
    public static class AsyncValidationExtensions
    {
        /// <summary>
        /// Defines an async validator on the current rule builder using a function that returns a Task&lt;bool&gt;
        /// </summary>
        /// <typeparam name="T">Type of object being validated</typeparam>
        /// <typeparam name="TProperty">Type of property being validated</typeparam>
        /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
        /// <param name="asyncPredicate">The async predicate to validate with</param>
        /// <param name="errorMessage">The error message to use if validation fails</param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, TProperty> MustAsync<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder,
            Func<T, TProperty, CancellationToken, Task<bool>> asyncPredicate,
            string errorMessage)
        {
            return ruleBuilder.MustAsync(asyncPredicate).WithMessage(errorMessage);
        }

        /// <summary>
        /// Defines an async validator on the current rule builder using a function that returns a Task&lt;bool&gt;
        /// </summary>
        /// <typeparam name="T">Type of object being validated</typeparam>
        /// <typeparam name="TProperty">Type of property being validated</typeparam>
        /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
        /// <param name="asyncPredicate">The async predicate to validate with</param>
        /// <param name="errorMessageFunc">Function to get the error message if validation fails</param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, TProperty> MustAsync<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder,
            Func<T, TProperty, CancellationToken, Task<bool>> asyncPredicate,
            Func<T, TProperty, string> errorMessageFunc)
        {
            return ruleBuilder.MustAsync(asyncPredicate).WithMessage(errorMessageFunc);
        }
    }
}
