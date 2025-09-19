using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.Car;
using CarRentalSystem.Domain.Entities;
using CarRentalSystem.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Infrastructure.Persistence.Repositories
{
    public class EfCarRepository : ICarRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public EfCarRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Guid> AddCarAsync(Car car, CancellationToken cancellationToken)
        {
            await _context.Cars.AddAsync(car, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return car.CarId;
        }

        public async Task<Car?> GetCarByIdAsync(Guid carId, CancellationToken cancellationToken)
        {
            return await _context.Cars
                .Include(c => c.CarApprovalStatus)
                .FirstOrDefaultAsync(c => c.CarId == carId, cancellationToken);
        }

        public async Task<List<Car>> GetAllCarsAsync(CancellationToken cancellationToken)
        {
            return await _context.Cars.ToListAsync(cancellationToken);
        }

        public async Task<List<Car>> GetCarsByOwnerAsync(Guid ownerId, CancellationToken cancellationToken)
        {
            return await _context.Cars
                .Include(c => c.CarApprovalStatus)
                .Where(c => c.OwnerId == ownerId)
                .ToListAsync(cancellationToken);
        }
        public async Task<List<Car>> GetAllCarsWithStatusAsync(CancellationToken cancellationToken)
        {
            return await _context.Cars
                .Include(c => c.CarApprovalStatus)
                .ToListAsync(cancellationToken);
        }
        public async Task<Guid> GetStatusIdByNameAsync(string statusName, CancellationToken cancellationToken)
        {
            var statusId = await _context.CarApprovalStatuses
                .Where(s => s.Name == statusName)
                .Select(s => s.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (statusId == Guid.Empty)
                throw new Exception("Status not found");

            return statusId;
        }



        public async Task UpdateCarAsync(Car car, CancellationToken cancellationToken)
        {
            _context.Cars.Update(car);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteCarAsync(Guid carId, CancellationToken cancellationToken)
        {
            var car = await _context.Cars.FindAsync(new object[] { carId }, cancellationToken);
            if (car != null)
            {
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<Guid> GetPendingApprovalStatusIdAsync(CancellationToken cancellationToken)
        {
            var statusId = await _context.CarApprovalStatuses
                .Where(s => s.Name == "Pending")
                .Select(s => s.Id) 
                .FirstOrDefaultAsync(cancellationToken);

                    if (statusId == Guid.Empty)
                        throw new Exception("Pending approval status not found.");

            return statusId;
        }

        public async Task<List<Car>> GetCarsByApprovalStatusAsync(string statusName, CancellationToken cancellationToken)
        {
            var statusId = await _context.CarApprovalStatuses
                .Where(s => s.Name == statusName)
                .Select(s => s.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (statusId == Guid.Empty)
                return new List<Car>();

            return await _context.Cars
                .Include(c => c.CarApprovalStatus)
                .Where(c => c.CarApprovalStatusId == statusId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<CarDto>> GetCarsByApprovalStatusWithOwnerAsync(string statusName, CancellationToken cancellationToken)
        {
            var statusId = await _context.CarApprovalStatuses
                .Where(s => s.Name == statusName)
                .Select(s => s.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (statusId == Guid.Empty)
                return new List<CarDto>();

            var cars = await _context.Cars
                .Include(c => c.CarApprovalStatus)
                .Where(c => c.CarApprovalStatusId == statusId)
                .ToListAsync(cancellationToken);

            var carDtos = new List<CarDto>();
            foreach (var car in cars)
            {
                var owner = await _context.Users
                    .Where(u => u.Id == car.OwnerId)
                    .Select(u => new { u.FullName, u.Email })
                    .FirstOrDefaultAsync(cancellationToken);

                var carDto = _mapper.Map<CarDto>(car);
                carDto.OwnerName = owner?.FullName ?? "Unknown";
                carDto.OwnerEmail = owner?.Email ?? "Unknown";
                carDtos.Add(carDto);
            }

            return carDtos;
        }
        public async Task<List<CarDto>> GetTopAvailableCarsAsync(int count = 6, CancellationToken cancellationToken = default)
        {
            return await _context.Cars
                .Include(c => c.CarApprovalStatus)
                .Where(c =>
                    c.CarApprovalStatus.Name == "Approved" &&
                    c.AvailableFrom <= DateTime.UtcNow &&
                    c.AvailableTo >= DateTime.UtcNow)
                .OrderByDescending(c => c.AvailableFrom)
                .ProjectTo<CarDto>(_mapper.ConfigurationProvider)
                .Take(count)
                .ToListAsync(cancellationToken);
        }


    }
}
