using CarRentalSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Common.Interfaces
{
    public interface IOTPRepository
    {
        Task<OTP?> GetValidOTPAsync(string email, string purpose);
        Task<OTP?> GetValidOTPAsync(string email, string code, string purpose);
        Task<OTP> CreateOTPAsync(OTP otp);
        Task<bool> MarkOTPAsUsedAsync(int otpId);
        Task<bool> DeleteExpiredOTPsAsync();
        Task<bool> DeleteOTPsForEmailAsync(string email, string purpose);
        Task<OTP?> GetLatestOTPAsync(string email, string purpose);
    }
}
