using System;
using System.Threading.Tasks;
using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;

namespace FlyingDutchmanAirlines.RepositoryLayer {
    public class BookingRepository {
        private readonly FlyingDutchmanAirlinesContext _context;

        public BookingRepository(FlyingDutchmanAirlinesContext context) {
            _context = context;
        }

        public async Task CreateBooking(int customerId, int flightNumber) {
            if (!customerId.IsPositive() || !flightNumber.IsPositive()) {
                Console.WriteLine($"Argument Exception in CreateBooking! CustomerID={customerId}," +
                                  $"flightNumber = {flightNumber}");
                throw new ArgumentException("Invalid arguments provided");
            }

            Booking newBooking = new Booking {
                CustomerId = customerId,
                FlightNumber = flightNumber
            };

            try {
                _context.Bookings.Add(newBooking);
                await _context.SaveChangesAsync();
            } catch (Exception exception) {
                Console.WriteLine($"Exception during database query: {exception.Message}");
                throw new CouldNotAddBookingToDatabaseExceptionException();
            }
        }
    }
}