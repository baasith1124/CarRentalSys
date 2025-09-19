using CarRentalSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Infrastructure.Persistence.Seeders
{
    public static class StatusSeeder
    {
        public static async Task SeedStatusesAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await context.Database.MigrateAsync();

            if (!await context.BookingStatuses.AnyAsync())
            {
                var bookingStatuses = new List<BookingStatus>
                {
                    new() { Id = Guid.NewGuid(), Name = "Pending" },
                    new() { Id = Guid.NewGuid(), Name = "Approved" },
                    new() { Id = Guid.NewGuid(), Name = "Confirmed" },
                    new() { Id = Guid.NewGuid(), Name = "Completed" },
                    new() { Id = Guid.NewGuid(), Name = "Cancelled" }
                };
                await context.BookingStatuses.AddRangeAsync(bookingStatuses);
            }

            if (!await context.PaymentStatuses.AnyAsync())
            {
                var paymentStatuses = new List<PaymentStatus>
                {
                    new() { Id = Guid.NewGuid(), Name = "Unpaid" },
                    new() { Id = Guid.NewGuid(), Name = "Paid" },
                    new() { Id = Guid.NewGuid(), Name = "Refunded" }
                };
                await context.PaymentStatuses.AddRangeAsync(paymentStatuses);
            }

            if (!await context.CarApprovalStatuses.AnyAsync())
            {
                var carStatuses = new List<CarApprovalStatus>
                {
                    new() { Id = Guid.NewGuid(), Name = "Pending" },
                    new() { Id = Guid.NewGuid(), Name = "Approved" },
                    new() { Id = Guid.NewGuid(), Name = "Rejected" }
                };
                await context.CarApprovalStatuses.AddRangeAsync(carStatuses);
            }

            await context.SaveChangesAsync();
        }
    }
}
