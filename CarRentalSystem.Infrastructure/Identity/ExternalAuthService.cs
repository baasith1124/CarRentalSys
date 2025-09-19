using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.Account;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Infrastructure.Identity;
using CarRentalSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace CarRentalSystem.Infrastructure.Identity
{
    public class ExternalAuthService : IExternalAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public ExternalAuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<bool> HandleExternalLoginAsync(ExternalUserModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    FullName = model.FullName,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded) return false;

                await _userManager.AddToRoleAsync(user, "Customer");
            }

            var loginExists = await _userManager.FindByLoginAsync(model.LoginProvider, model.ProviderKey);
            if (loginExists == null)
            {
                var loginInfo = new UserLoginInfo(model.LoginProvider, model.ProviderKey, model.LoginProvider);
                var resultLogin = await _userManager.AddLoginAsync(user, loginInfo);
                if (!resultLogin.Succeeded) return false;
            }

            // Add to Customers table if not exists
            var existingCustomer = await _context.Customers.FindAsync(user.Id);
            if (existingCustomer == null)
            {
                var customer = new Customer
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    ProfileImagePath = user.ProfileImagePath,
                    NIC = user.NICNumber,
                    Address = user.Address
                };

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return true;
        }
    }
}
