using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Contracts.Account
{
    public class ExternalUserModel
    {
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string LoginProvider { get; set; } = null!;
        public string ProviderKey { get; set; } = null!;
    }
}
