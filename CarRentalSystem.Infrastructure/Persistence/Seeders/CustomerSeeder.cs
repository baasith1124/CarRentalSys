using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Infrastructure.Persistence.Seeders
{
    public static class CustomerSeeder
    {
        public static async Task SeedCustomersFromIdentityAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Get all users with Customer role only
            var customerUsers = await userManager.GetUsersInRoleAsync("Customer");

            foreach (var user in customerUsers)
            {
                bool customerExists = await context.Customers.AnyAsync(c => c.Id == user.Id);
                if (!customerExists)
                {
                    var customer = new Customer
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        NIC = user.NICNumber,
                        ProfileImagePath = user.ProfileImagePath,
                        Address = user.Address
                    };

                    await context.Customers.AddAsync(customer);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
