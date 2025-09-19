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
    public static class UpdateBookingStatuses
    {
        public static async Task AddMissingBookingStatusesAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Check if Confirmed status exists
            var confirmedExists = await context.BookingStatuses
                .AnyAsync(s => s.Name == "Confirmed");

            if (!confirmedExists)
            {
                var confirmedStatus = new BookingStatus
                {
                    Id = Guid.NewGuid(),
                    Name = "Confirmed"
                };
                await context.BookingStatuses.AddAsync(confirmedStatus);
                Console.WriteLine("Added 'Confirmed' booking status");
            }

            // Check if Completed status exists
            var completedExists = await context.BookingStatuses
                .AnyAsync(s => s.Name == "Completed");

            if (!completedExists)
            {
                var completedStatus = new BookingStatus
                {
                    Id = Guid.NewGuid(),
                    Name = "Completed"
                };
                await context.BookingStatuses.AddAsync(completedStatus);
                Console.WriteLine("Added 'Completed' booking status");
            }

            await context.SaveChangesAsync();
            Console.WriteLine("Booking statuses updated successfully");
        }
    }
}
