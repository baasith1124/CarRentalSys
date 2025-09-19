using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Infrastructure.Persistence.Seeders
{
    public static class SeederRunner
    {
        public static async Task RunAllSeedersAsync(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("SeederRunner");

            try
            {
                logger.LogInformation(" Running Database Seeders...");

                await IdentitySeeder.SeedRolesAndAdminAsync(serviceProvider);
                logger.LogInformation(" IdentitySeeder completed.");

                await StatusSeeder.SeedStatusesAsync(serviceProvider);
                logger.LogInformation(" StatusSeeder completed.");

                await UpdateBookingStatuses.AddMissingBookingStatusesAsync(serviceProvider);
                logger.LogInformation(" UpdateBookingStatuses completed.");

                await CustomerSeeder.SeedCustomersFromIdentityAsync(serviceProvider);
                logger.LogInformation(" CustomerSeeder completed.");

                var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
                await CarSeeder.SeedAsync(context);
                logger.LogInformation("CarSeeder completed.");

                logger.LogInformation(" All seeders executed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, " Seeder execution failed: {Message}", ex.Message);
                throw; // Rethrow so app startup fails if seed fails
            }
        }
    }
}
