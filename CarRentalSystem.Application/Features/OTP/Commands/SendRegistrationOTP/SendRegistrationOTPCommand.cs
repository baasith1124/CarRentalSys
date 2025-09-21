using CarRentalSystem.Application.Contracts.OTP;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.OTP.Commands.SendRegistrationOTP
{
    public class SendRegistrationOTPCommand : IRequest<OTPResponse>
    {
        public string Email { get; set; } = string.Empty;
    }
}

