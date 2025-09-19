using CarRentalSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        IQueryable<Car> Cars { get; }
        IQueryable<CarApprovalStatus> CarApprovalStatuses { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
