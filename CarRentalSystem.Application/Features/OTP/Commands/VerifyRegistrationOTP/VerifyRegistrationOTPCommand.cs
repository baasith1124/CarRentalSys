using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.OTP.Commands.VerifyRegistrationOTP
{
    public class VerifyRegistrationOTPCommand : IRequest<bool>
    {
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Purpose { get; set; } = "Registration";
    }
}
