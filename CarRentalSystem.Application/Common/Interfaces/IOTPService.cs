using CarRentalSystem.Application.Contracts.OTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Common.Interfaces
{
    public interface IOTPService
    {
        Task<OTPResponse> GenerateAndSendOTPAsync(OTPRequest request);
        Task<bool> VerifyOTPAsync(OTPVerificationRequest request);
        Task<bool> MarkOTPAsUsedAsync(string email, string purpose);
        Task<OTPResponse> ResendOTPAsync(OTPRequest request);
        Task CleanupExpiredOTPsAsync();
    }
}
