using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FlyingDutchmanAirlines.RepositoryLayer
{
    public class FlightRepository
    {
        private readonly FlyingDutchmanAirlinesContext _context;

        public FlightRepository(FlyingDutchmanAirlinesContext context)
        {
            _context = context;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public FlightRepository()
        {
            if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
            {
                throw new Exception("This constructor should only be used for testing");
            }
        }

        public async Task<Flight> GetFlightByFlightNumber(int flightNumber, int originAirportId,
            int destinationAirportId)
        {
            if (!flightNumber.IsPositive())
            {
                Console.WriteLine($"Could not find flight in GetFlightByFlightNumber! flightNumber = {flightNumber}");
                throw new FlightNotFoundException();
            }
            
            if (!originAirportId.IsPositive() || !destinationAirportId.IsPositive())
            {
                Console.WriteLine($"Argument Exception in GetFlightByFlightnumber! originAirportId = {originAirportId} :" +
                                  $"destinationAirportId = {destinationAirportId}");
                throw new ArgumentException("invalid argument provided");
            }

            return await _context.Flights.FirstOrDefaultAsync(f => f.FlightNumber == flightNumber)
                   ?? throw new FlightNotFoundException();

        }
        

        public virtual async Task<Flight> GetFlightByFlightNumber(int flightNumber)
        {
            if (!flightNumber.IsPositive())
            {
                Console.WriteLine($"Could not find flight in GetFlightByFlightNumber! flightNumber = {flightNumber}");
                throw new FlightNotFoundException();
            }

            return await _context.Flights.FirstOrDefaultAsync(f => f.FlightNumber == flightNumber) ??
                   throw new FlightNotFoundException();
        }

        public virtual Queue<Flight> GetFlights()
        // public virtual IEnumerable<Flight> GetFlights()
        {
            Queue<Flight> flights = new Queue<Flight>();
            foreach (Flight flight in _context.Flights)
            {
                flights.Enqueue(flight);
                // yield return flight;
            }
            // _context.Flights.ForEachAsync(f => flights.Enqueue(f));

            return flights;
        }

    }
}