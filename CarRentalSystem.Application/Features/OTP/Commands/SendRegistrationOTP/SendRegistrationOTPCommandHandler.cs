using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.OTP;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.OTP.Commands.SendRegistrationOTP
{
    public class SendRegistrationOTPCommandHandler : IRequestHandler<SendRegistrationOTPCommand, OTPResponse>
    {
        private readonly IOTPService _otpService;

        public SendRegistrationOTPCommandHandler(IOTPService otpService)
        {
            _otpService = otpService;
        }

        public async Task<OTPResponse> Handle(SendRegistrationOTPCommand request, CancellationToken cancellationToken)
        {
            var otpRequest = new OTPRequest
            {
                Email = request.Email,
                Purpose = "Registration"
            };

            return await _otpService.GenerateAndSendOTPAsync(otpRequest);
        }
    }
}

