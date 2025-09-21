using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.Account;
using CarRentalSystem.Application.Contracts.OTP;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Account.Commands.RegisterCustomerWithOTP
{
    public class RegisterCustomerWithOTPCommandHandler : IRequestHandler<RegisterCustomerWithOTPCommand, bool>
    {
        private readonly IIdentityService _identityService;
        private readonly IOTPService _otpService;

        public RegisterCustomerWithOTPCommandHandler(IIdentityService identityService, IOTPService otpService)
        {
            _identityService = identityService;
            _otpService = otpService;
        }

        public async Task<bool> Handle(RegisterCustomerWithOTPCommand request, CancellationToken cancellationToken)
        {
            // OTP verification is handled separately, so we just proceed with registration
            if (request.Password != request.ConfirmPassword)
                throw new ArgumentException("Passwords do not match.");

            var user = new RegisterUserModel
            {
                FullName = request.FullName,
                Email = request.Email,
                Password = request.Password
            };

            var registrationResult = await _identityService.RegisterCustomerAsync(user);
            
            // If registration is successful, mark the OTP as used
            if (registrationResult)
            {
                await _otpService.MarkOTPAsUsedAsync(request.Email, "Registration");
                return true;
            }
            else
            {
                // Registration failed - likely due to duplicate user
                throw new InvalidOperationException($"Registration failed. The email '{request.Email}' is already registered. Please use a different email or try logging in instead.");
            }
        }
    }
}
