using CarRentalSystem.Application.Common.Interfaces;
using FluentValidation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Common.Validators
{
    public class CarAvailabilityRequest
    {
        public Guid CarId { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTime ReturnDate { get; set; }
    }

    public class AsyncCarAvailabilityValidator : AbstractValidator<CarAvailabilityRequest>
    {
        private readonly IBookingRepository _bookingRepository;

        public AsyncCarAvailabilityValidator(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public override async Task<FluentValidation.Results.ValidationResult> ValidateAsync(
            ValidationContext<CarAvailabilityRequest> context, 
            CancellationToken cancellation = default)
        {
            var result = new FluentValidation.Results.ValidationResult();

            if (context.InstanceToValidate != null)
            {
                var request = context.InstanceToValidate;
                
                // Async validation: Check if car is available
                var isAvailable = await _bookingRepository.IsCarAvailableAsync(
                    request.CarId, 
                    request.PickupDate, 
                    request.ReturnDate, 
                    cancellation);

                if (!isAvailable)
                {
                    result.Errors.Add(new FluentValidation.Results.ValidationFailure(
                        nameof(CarAvailabilityRequest.CarId),
                        "This car is already booked for the selected period."));
                }
            }

            return result;
        }
    }
}
