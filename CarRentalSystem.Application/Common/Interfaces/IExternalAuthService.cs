using CarRentalSystem.Application.Contracts.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Common.Interfaces
{
    public interface IExternalAuthService
    {
        Task<bool> HandleExternalLoginAsync(ExternalUserModel model);
    }
}
