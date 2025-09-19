using CarRentalSystem.Application.Contracts.Account;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Account.Commands.ExternalLogin
{
    public class ExternalLoginCommand : IRequest<bool>
    {
        public ExternalUserModel User { get; set; } = null!;
    }
}
