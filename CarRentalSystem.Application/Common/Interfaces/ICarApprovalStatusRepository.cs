using CarRentalSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Common.Interfaces
{
    public interface ICarApprovalStatusRepository
    {
        Task<List<CarApprovalStatus>> GetAllStatusesAsync(CancellationToken cancellationToken);
        Task<CarApprovalStatus?> GetStatusByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> UpdateStatusAsync(Guid carId, Guid newStatusId, CancellationToken cancellationToken);
    }
}
