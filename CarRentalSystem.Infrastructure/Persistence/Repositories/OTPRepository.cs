using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Infrastructure.Persistence.Repositories
{
    public class OTPRepository : IOTPRepository
    {
        private readonly ApplicationDbContext _context;

        public OTPRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OTP?> GetValidOTPAsync(string email, string purpose)
        {
            return await _context.OTPs
                .Where(o => o.Email == email && 
                           o.Purpose == purpose && 
                           !o.IsUsed && 
                           o.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<OTP?> GetValidOTPAsync(string email, string code, string purpose)
        {
            return await _context.OTPs
                .Where(o => o.Email == email && 
                           o.Code == code &&
                           o.Purpose == purpose && 
                           !o.IsUsed && 
                           o.ExpiresAt > DateTime.UtcNow)
                .FirstOrDefaultAsync();
        }

        public async Task<OTP> CreateOTPAsync(OTP otp)
        {
            _context.OTPs.Add(otp);
            await _context.SaveChangesAsync();
            return otp;
        }

        public async Task<bool> MarkOTPAsUsedAsync(int otpId)
        {
            var otp = await _context.OTPs.FindAsync(otpId);
            if (otp == null) return false;

            otp.IsUsed = true;
            otp.UsedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteExpiredOTPsAsync()
        {
            var expiredOTPs = await _context.OTPs
                .Where(o => o.ExpiresAt <= DateTime.UtcNow)
                .ToListAsync();

            if (expiredOTPs.Any())
            {
                _context.OTPs.RemoveRange(expiredOTPs);
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> DeleteOTPsForEmailAsync(string email, string purpose)
        {
            var otps = await _context.OTPs
                .Where(o => o.Email == email && o.Purpose == purpose)
                .ToListAsync();

            if (otps.Any())
            {
                _context.OTPs.RemoveRange(otps);
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<OTP?> GetLatestOTPAsync(string email, string purpose)
        {
            return await _context.OTPs
                .Where(o => o.Email == email && o.Purpose == purpose)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
        }
    }
}
