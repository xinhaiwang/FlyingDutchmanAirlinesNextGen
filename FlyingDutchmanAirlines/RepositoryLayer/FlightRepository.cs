using System;
using System.Diagnostics;
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

    }
}