using System;
using System.Linq;
using System.Threading.Tasks;
using FlyingDutchmanAirlines_Tests.Stubs;
using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.RepositoryLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlyingDutchmanAirlines_Tests.RepositoryLayer
{
    public class FlightRepositoryTests
    {
        private FlyingDutchmanAirlinesContext _context;
        private FlightRepository _repository;

        [TestInitialize]
        public async Task TestInitialize()
        {
            DbContextOptions<FlyingDutchmanAirlinesContext> dbContextOptions =
                new DbContextOptionsBuilder<FlyingDutchmanAirlinesContext>().UseInMemoryDatabase("FlyingDutchman")
                    .Options;
            _context = new FlyingDutchmanAirlinesContext_Stub(dbContextOptions);

            Flight flight = new Flight
            {
                FlightNumber = 1,
                Origin = 1,
                Destination = 2
            };

            Flight flight2 = new Flight
            {
                FlightNumber = 10,
                Origin = 3,
                Destination = 4
            };

            _context.Flights.Add(flight);
            _context.Flights.Add(flight2);
            await _context.SaveChangesAsync();

            _repository = new FlightRepository(_context);
            Assert.IsNotNull(_repository);
        }

        [TestMethod]
        [ExpectedException(typeof(FlightNotFoundException))]
        public async Task GetFlightByFlightNumber_Failure_InvalidFlightNumber()
        {
            await _repository.GetFlightByFlightNumber(-1, 0, 0);
        }
        
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetFlightByFlightNumber_Failure_InvalidOriginAirportId()
        {
            await _repository.GetFlightByFlightNumber(0, -1, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetFlightByFlightNumber_Failure_InvalidDestinationAirportId()
        {
            await _repository.GetFlightByFlightNumber(0, 0, -1);
        }

        [TestMethod]
        public async Task GetFlightNumber_Success()
        {
            Flight flight = await _repository.GetFlightByFlightNumber(1, 1, 2);
            Assert.IsNotNull(flight);

            Flight dbFlight = _context.Flights.First(f => f.FlightNumber == 1);
            Assert.IsNotNull(dbFlight);
            
            Assert.AreEqual(dbFlight.FlightNumber, flight.FlightNumber);
            Assert.AreEqual(dbFlight.Origin, flight.Origin);
            Assert.AreEqual(dbFlight.Destination, flight.Destination);
        }
        
        [TestMethod]
        [ExpectedException(typeof(FlightNotFoundException))]
        public async Task GetFlightByFlightNumber_Failure_DatabaseException()
        {
            await _repository.GetFlightByFlightNumber(2, 1, 2);
        }
    }
}