using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.ServiceLayer;
using FlyingDutchmanAirlines.Views;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FlyingDutchmanAirlines_Tests.ServiceLayer
{
    [TestClass]
    public class FlightServiceTests
    {
        private Mock<FlightRepository> _mockFlightRepository;
        private Mock<AirportRepository> _mockAirportRepository;

        [TestInitialize]
        public void Initialize()
        {
            _mockFlightRepository = new Mock<FlightRepository>();
            _mockAirportRepository = new Mock<AirportRepository>();

            _mockAirportRepository.Setup(repository => repository.GetAirportById(31))
                .ReturnsAsync(new Airport
                {
                    AirportId = 31,
                    City = "Mexico City",
                    Iata = "MEX"
                });

            _mockAirportRepository.Setup(repository => repository.GetAirportById(92)).ReturnsAsync(new Airport
            {
                AirportId = 92,
                City = "Ulaanbaatar",
                Iata = "UBN"
            });

            Flight flightInDatabase = new Flight
            {
                FlightNumber = 148,
                Origin = 31,
                Destination = 92
            };

            Queue<Flight> mockReturn = new Queue<Flight>(1);
            mockReturn.Enqueue(flightInDatabase);

            _mockFlightRepository.Setup(repository => repository.GetFlights()).Returns(mockReturn);
            _mockFlightRepository.Setup(repository => repository.GetFlightByFlightNumber(148))
                .Returns(Task.FromResult(flightInDatabase));
        }

        [TestMethod]
        public async Task GetFlights_Success()
        {
            FlightService service = new FlightService(_mockFlightRepository.Object, _mockAirportRepository.Object);

            //Instead of calling the FlightService.GetFlights method once, waiting for all the data to come back,
            // and then operating on it, the IAsyncEnumerable type allows us to await on a foreach loop and operate
            // on the returned data as it comes in.
            await foreach (FlightView flightView in service.GetFlights())
            {
                Assert.IsNotNull(flightView);
                Assert.AreEqual(flightView.FlightNumber, "148");
                Assert.AreEqual(flightView.Origin.City, "Mexico City");
                Assert.AreEqual(flightView.Origin.Code, "MEX");
                Assert.AreEqual(flightView.Destination.City, "Ulaanbaatar");
                Assert.AreEqual(flightView.Destination.Code, "UBN");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FlightNotFoundException))]
        public async Task GetFlights_Failure_RepositoryException()
        {
            _mockAirportRepository.Setup(repository => repository.GetAirportById(31))
                .ThrowsAsync(new FlightNotFoundException());

            FlightService service = new FlightService(_mockFlightRepository.Object, _mockAirportRepository.Object);
            await foreach (FlightView _ in service.GetFlights())
            {
                // Empty statement.
                ;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetFlights_Failure_RegularException()
        {
            _mockAirportRepository.Setup(repository => repository.GetAirportById(31))
                .ThrowsAsync(new NullReferenceException());

            FlightService service = new FlightService(_mockFlightRepository.Object, _mockAirportRepository.Object);
            await foreach (FlightView _ in service.GetFlights())
            {
                ;
            }
        }

        [TestMethod]
        public async Task GetFlightByFlightNumber_Success()
        {
            FlightService service = new FlightService(_mockFlightRepository.Object, _mockAirportRepository.Object);
            FlightView flightView = await service.GetFlightByFlightNumber(148);
            
            Assert.IsNotNull(flightView);
            Assert.AreEqual(flightView.FlightNumber, "148");
            Assert.AreEqual(flightView.Origin.City, "Mexico City");
            Assert.AreEqual(flightView.Origin.Code, "MEX");
            Assert.AreEqual(flightView.Destination.City, "Ulaanbaatar");
            Assert.AreEqual(flightView.Destination.Code, "UBN");
        }

        [TestMethod]
        [ExpectedException(typeof(FlightNotFoundException))]
        public async Task GetFlightByFlightNumber_Failure_RepositoryException_FlightNotFoundException()
        {
            _mockFlightRepository.Setup(repository => repository.GetFlightByFlightNumber(-1))
                .Throws(new FlightNotFoundException());
            FlightService service = new FlightService(_mockFlightRepository.Object, _mockAirportRepository.Object);

            await service.GetFlightByFlightNumber(-1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetFlightByFlightNumber_Failure_RepositoryException_Exception()
        {
            _mockFlightRepository.Setup(repository => repository.GetFlightByFlightNumber(-1))
                .Throws(new OverflowException());
            FlightService service = new FlightService(_mockFlightRepository.Object, _mockAirportRepository.Object);

            await service.GetFlightByFlightNumber(-1);
        }
        

    }
}