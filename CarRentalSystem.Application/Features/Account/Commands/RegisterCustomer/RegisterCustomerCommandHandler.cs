﻿using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.Account;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Account.Commands.RegisterCustomer
{
    public class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand, bool>
    {
        private readonly IIdentityService _identityService;

        public RegisterCustomerCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<bool> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
        {
            if (request.Password != request.ConfirmPassword)
                throw new ArgumentException("Passwords do not match.");

            var user = new RegisterUserModel
            {
                FullName = request.FullName,
                Email = request.Email,
                Password = request.Password
            };

            return await _identityService.RegisterCustomerAsync(user);
        }
    }
}
