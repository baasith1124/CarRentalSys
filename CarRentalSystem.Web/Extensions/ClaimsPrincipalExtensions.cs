﻿using System.Security.Claims;

namespace CarRentalSystem.Web.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            // Check if user is authenticated first
            if (!user.Identity?.IsAuthenticated == true)
                throw new UnauthorizedAccessException("User not authenticated");
            
            // Try to get user ID from the standard ASP.NET Core Identity claim
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
                return userId;
            
            // Fallback to "sub" claim for JWT scenarios
            var subClaim = user.FindFirst("sub");
            if (subClaim != null && Guid.TryParse(subClaim.Value, out var subUserId))
                return subUserId;
            
            throw new InvalidOperationException("Unable to retrieve user ID from claims");
        }
        
        public static Guid? GetUserIdOrNull(this ClaimsPrincipal user)
        {
            if (!user.Identity?.IsAuthenticated == true)
                return null;
            
            // Try to get user ID from the standard ASP.NET Core Identity claim
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
                return userId;
            
            // Fallback to "sub" claim for JWT scenarios
            var subClaim = user.FindFirst("sub");
            if (subClaim != null && Guid.TryParse(subClaim.Value, out var subUserId))
                return subUserId;
            
            return null;
        }
    }
}
