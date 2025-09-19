using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Contracts.CarApprovalStatus
{
    public class CarApprovalStatusDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
