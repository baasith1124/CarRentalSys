using CarRentalSystem.Application.Contracts.Car;
using CarRentalSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Common.Interfaces
{
    public interface ICarRepository
    {
        // Create
        Task<Guid> AddCarAsync(Car car, CancellationToken cancellationToken);

        // Read
        Task<Car?> GetCarByIdAsync(Guid carId, CancellationToken cancellationToken);
        Task<List<Car>> GetAllCarsAsync(CancellationToken cancellationToken);
        Task<List<Car>> GetCarsByOwnerAsync(Guid ownerId, CancellationToken cancellationToken);
        Task<List<Car>> GetAllCarsWithStatusAsync(CancellationToken cancellationToken);
        Task<Guid> GetStatusIdByNameAsync(string statusName, CancellationToken cancellationToken);

        // Update
        Task UpdateCarAsync(Car car, CancellationToken cancellationToken);

        // Delete
        Task DeleteCarAsync(Guid carId, CancellationToken cancellationToken);

        // Status
        Task<Guid> GetPendingApprovalStatusIdAsync(CancellationToken cancellationToken);
        Task<List<Car>> GetCarsByApprovalStatusAsync(string statusName, CancellationToken cancellationToken);
        Task<List<CarDto>> GetCarsByApprovalStatusWithOwnerAsync(string statusName, CancellationToken cancellationToken);
        Task<List<CarDto>> GetTopAvailableCarsAsync(int count = 6, CancellationToken cancellationToken = default);


    }
}
