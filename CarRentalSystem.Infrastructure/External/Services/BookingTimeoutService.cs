using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CarRentalSystem.Infrastructure.External.Services
{
    public class BookingTimeoutService : BackgroundService, IBookingTimeoutService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BookingTimeoutService> _logger;
        private readonly TimeSpan _timeoutPeriod = TimeSpan.FromMinutes(15); // 15 minutes timeout

        public BookingTimeoutService(IServiceProvider serviceProvider, ILogger<BookingTimeoutService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Booking Timeout Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessBookingTimeoutsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing booking timeouts");
                }

                // Run every 5 minutes
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }

            _logger.LogInformation("Booking Timeout Service stopped");
        }

        public async Task ProcessBookingTimeoutsAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();

            try
            {
                var cutoffTime = DateTime.UtcNow.Subtract(_timeoutPeriod);
                
                // Get expired unpaid bookings
                var expiredBookings = await bookingRepository.GetExpiredUnpaidBookingsAsync(cutoffTime, cancellationToken);

                if (expiredBookings.Any())
                {
                    _logger.LogInformation($"Found {expiredBookings.Count} expired bookings to cancel");

                    foreach (var booking in expiredBookings)
                    {
                        try
                        {
                            await bookingRepository.CancelBookingAsync(booking.BookingId, cancellationToken);
                            _logger.LogInformation($"Cancelled expired booking {booking.BookingId}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Failed to cancel booking {booking.BookingId}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing booking timeouts");
            }
        }
    }
}
