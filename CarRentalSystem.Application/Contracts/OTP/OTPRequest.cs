using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Contracts.OTP
{
    public class OTPRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public string? __RequestVerificationToken { get; set; }
    }

    public class OTPVerificationRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public string? __RequestVerificationToken { get; set; }
    }

    public class OTPResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime? ExpiresAt { get; set; }
        public int ResendCooldownSeconds { get; set; }
    }
}
