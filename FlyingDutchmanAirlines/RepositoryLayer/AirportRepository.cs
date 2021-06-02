using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FlyingDutchmanAirlines.RepositoryLayer {
    public class AirportRepository {
        private readonly FlyingDutchmanAirlinesContext _context;
        public AirportRepository(FlyingDutchmanAirlinesContext context) {
            _context = context;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public AirportRepository()
        {
            if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
            {
                throw new Exception("This constructor should only be used for testing");
            }
        }

        public virtual async Task<Airport> GetAirportById(int airportId) {
            if (!airportId.IsPositive()) {
                Console.WriteLine($"Argument Exception in GetAirportByID! AirportID = {airportId}");
                throw new ArgumentException("invalid argument provided");
            }

            return await _context.Airports.FirstOrDefaultAsync(a => a.AirportId == airportId)
                   ?? throw new AirportNotFoundException();
        }
        
    }
}