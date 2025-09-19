﻿using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.Account;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Infrastructure.Persistence.Repositories
{
    public class EfIdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ApplicationDbContext _context;

        public EfIdentityService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<bool> RegisterCustomerAsync(ApplicationUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded) return false;

           
            await _userManager.AddToRoleAsync(user, "Customer");

            return true;
        }

        public async Task<bool> RegisterCustomerAsync(RegisterUserModel model)
        {
            var user = new ApplicationUser
            {
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.Email, // Use email as username for consistency
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"Identity error: {error.Code} - {error.Description}");
                }
                return false;
            }
            Console.WriteLine($" User created. ID: {user.Id}");
            // Add to Customers table
            _context.Customers.Add(new Customer
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                NIC = user.NICNumber,
                Address = user.Address,
                ProfileImagePath = user.ProfileImagePath
            });

            await _context.SaveChangesAsync();

            await _userManager.AddToRoleAsync(user, model.Role ?? "Customer");

            return true;
        }

    }
}
