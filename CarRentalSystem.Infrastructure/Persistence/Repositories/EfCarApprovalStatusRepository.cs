using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Infrastructure.Persistence.Repositories
{
    public class EfCarApprovalStatusRepository : ICarApprovalStatusRepository
    {
        private readonly ApplicationDbContext _context;

        public EfCarApprovalStatusRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CarApprovalStatus>> GetAllStatusesAsync(CancellationToken cancellationToken)
        {
            return await _context.CarApprovalStatuses
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<CarApprovalStatus?> GetStatusByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.CarApprovalStatuses
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<bool> UpdateStatusAsync(Guid carId, Guid newStatusId, CancellationToken cancellationToken)
        {
            var car = await _context.Cars.FirstOrDefaultAsync(c => c.CarId == carId, cancellationToken);

            if (car == null)
                return false;

            car.CarApprovalStatusId = newStatusId;

            _context.Cars.Update(car);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
