using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Infrastructure.Persistence.Seeders
{
    public class BookingLocationSeeder
    {
        private readonly ApplicationDbContext _context;

        public BookingLocationSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedBookingLocationsAsync()
        {
            // Sample locations with coordinates for testing
            var sampleLocations = new[]
            {
                new { 
                    PickupLocation = "Times Square, New York, NY, USA", 
                    PickupLat = 40.7580, 
                    PickupLng = -73.9855,
                    DropLocation = "Central Park, New York, NY, USA",
                    DropLat = 40.7829,
                    DropLng = -73.9654
                },
                new { 
                    PickupLocation = "Golden Gate Bridge, San Francisco, CA, USA", 
                    PickupLat = 37.8199, 
                    PickupLng = -122.4783,
                    DropLocation = "Fisherman's Wharf, San Francisco, CA, USA",
                    DropLat = 37.8080,
                    DropLng = -122.4177
                },
                new { 
                    PickupLocation = "Hollywood Sign, Los Angeles, CA, USA", 
                    PickupLat = 34.1341, 
                    PickupLng = -118.3215,
                    DropLocation = "Santa Monica Pier, Santa Monica, CA, USA",
                    DropLat = 34.0089,
                    DropLng = -118.4973
                },
                new { 
                    PickupLocation = "Space Needle, Seattle, WA, USA", 
                    PickupLat = 47.6205, 
                    PickupLng = -122.3493,
                    DropLocation = "Pike Place Market, Seattle, WA, USA",
                    DropLat = 47.6097,
                    DropLng = -122.3331
                },
                new { 
                    PickupLocation = "Millennium Park, Chicago, IL, USA", 
                    PickupLat = 41.8826, 
                    PickupLng = -87.6226,
                    DropLocation = "Navy Pier, Chicago, IL, USA",
                    DropLat = 41.8919,
                    DropLng = -87.6089
                }
            };

            // Get all bookings that don't have location data
            var bookingsWithoutLocations = await _context.Bookings
                .Where(b => string.IsNullOrEmpty(b.PickupLocation) || string.IsNullOrEmpty(b.DropLocation))
                .ToListAsync();

            var random = new Random();
            int locationIndex = 0;

            foreach (var booking in bookingsWithoutLocations)
            {
                var location = sampleLocations[locationIndex % sampleLocations.Length];
                
                booking.PickupLocation = location.PickupLocation;
                booking.DropLocation = location.DropLocation;
                booking.PickupLatitude = location.PickupLat;
                booking.PickupLongitude = location.PickupLng;
                booking.DropLatitude = location.DropLat;
                booking.DropLongitude = location.DropLng;

                locationIndex++;
            }

            if (bookingsWithoutLocations.Any())
            {
                _context.Bookings.UpdateRange(bookingsWithoutLocations);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Updated {bookingsWithoutLocations.Count} bookings with location data.");
            }
            else
            {
                Console.WriteLine("No bookings found that need location data updates.");
            }
        }
    }
}
