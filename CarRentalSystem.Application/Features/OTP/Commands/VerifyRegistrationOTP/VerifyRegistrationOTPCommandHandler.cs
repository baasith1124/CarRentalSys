using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.OTP;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.OTP.Commands.VerifyRegistrationOTP
{
    public class VerifyRegistrationOTPCommandHandler : IRequestHandler<VerifyRegistrationOTPCommand, bool>
    {
        private readonly IOTPService _otpService;

        public VerifyRegistrationOTPCommandHandler(IOTPService otpService)
        {
            _otpService = otpService;
        }

        public async Task<bool> Handle(VerifyRegistrationOTPCommand request, CancellationToken cancellationToken)
        {
            var verificationRequest = new OTPVerificationRequest
            {
                Email = request.Email,
                Code = request.Code,
                Purpose = "Registration"
            };

            return await _otpService.VerifyOTPAsync(verificationRequest);
        }
    }
}

