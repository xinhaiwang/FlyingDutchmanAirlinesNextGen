using System;
using System.Reflection;
using System.Runtime.CompilerServices;
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

        [MethodImpl(MethodImplOptions.NoInlining)]
        public BookingRepository()
        {
            if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
            {
                throw new Exception("This constructor should only be used for testing");
            }
        }

        public virtual async Task CreateBooking(int customerId, int flightNumber) {
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
                throw new CouldNotAddBookingToDatabaseException();
            }
        }
    }
}