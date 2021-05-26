using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FlyingDutchmanAirlines_Tests.Stubs;
using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.RepositoryLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlyingDutchmanAirlines_Tests.RepositoryLayer {
    public class AirportRepositoryTests {
        private FlyingDutchmanAirlinesContext _context;
        private AirportRepository _repository;

        [TestInitialize]
        public async Task TestInitialize() {
            DbContextOptions<FlyingDutchmanAirlinesContext> dbContextOptions = new
                DbContextOptionsBuilder<FlyingDutchmanAirlinesContext>().UseInMemoryDatabase("FlyingDutchman").Options;
            _context = new FlyingDutchmanAirlinesContext_Stub(dbContextOptions);

            SortedList<string, Airport> airports = new SortedList<string, Airport> {
                {
                    "GOH",
                    new Airport {
                        AirportId = 0,
                        City = "Nuuk",
                        Iata = "GOH"
                    }
                },
                {
                    "PHX",
                    new Airport {
                        AirportId = 1,
                        City = "Phoenix",
                        Iata = "PHX"
                    }
                },
                {
                    "DDH",
                    new Airport {
                        AirportId = 2,
                        City = "Bennington",
                        Iata = "DDH"
                    }
                },
                {
                    "RDU",
                    new Airport {
                        AirportId = 3,
                        City = "Raleigh-Durham",
                        Iata = "RDu"
                    }
                }
            };

            _context.Airports.AddRange(airports.Values);
            await _context.SaveChangesAsync();

            _repository = new AirportRepository(_context);
            Assert.IsNotNull(_repository);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public async Task GetAirportById_Success(int airportId) {
            Airport airport = await _repository.GetAirportById(airportId);
            Assert.IsNotNull(airport);

            Airport dbAirport = _context.Airports.First(a => a.AirportId == airportId);
            
            Assert.AreEqual(dbAirport.AirportId, airport.AirportId);
            Assert.AreEqual("Nuuk", airport.City);
            Assert.AreEqual("GOH", airport.Iata);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetAirportByID_Failure_InvalidInput() {
            StringWriter outputStream = new StringWriter();
            try {
                    Console.SetOut(outputStream);
                    await _repository.GetAirportById(-1);
            } catch (ArgumentException) {
                Assert.IsTrue(outputStream.ToString().Contains("Argument Exception in GetAirportByID! AirportID = -1"));
                throw new AggregateException();
            } finally {
                await outputStream.DisposeAsync();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AirportNotFoundException))]
        public async Task GetAirportByID_Failure_DatabaseException() {
            await _repository.GetAirportById(10);
        }
    }
}