using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.Views;

namespace FlyingDutchmanAirlines.ServiceLayer
{
    public class FlightService
    {
        private readonly FlightRepository _flightRepository;
        private readonly AirportRepository _airportRepository;

        public FlightService(FlightRepository flightRepository, AirportRepository airportRepository)
        {
            _flightRepository = flightRepository;
            _airportRepository = airportRepository;
        }

        public async IAsyncEnumerable<FlightView> GetFlights()
        {
            Queue<Flight> flights = _flightRepository.GetFlights();
            foreach (Flight flight in flights)
            {
                Airport originAirport;
                Airport destinationAirport;

                try
                {
                    originAirport = await _airportRepository.GetAirportById(flight.Origin);
                    destinationAirport = await _airportRepository.GetAirportById(flight.Destination);
                } 
                catch (FlightNotFoundException)
                {
                    throw new FlightNotFoundException();
                } 
                catch (Exception)
                {
                    throw new ArgumentException();
                }

                // Because we return a type of IAsyncEnumerable<FlightView>, we can use the yield
                // return keywords to automatically add the created FlightView instances to a 
                // compiler-generated list.
                yield return new FlightView(flight.FlightNumber.ToString(),
                    (originAirport.City, originAirport.Iata),
                    (destinationAirport.City, destinationAirport.Iata));
            }
        }

        public virtual async Task<FlightView> GetFlightByFlightNumber(int flightNumber)
        {
            try
            {
                Flight flight = await _flightRepository.GetFlightByFlightNumber(flightNumber);
                Airport originAirport = await _airportRepository.GetAirportById(flight.Origin);
                Airport destinationAirport = await _airportRepository.GetAirportById(flight.Destination);

                return new FlightView(flight.FlightNumber.ToString(),
                    (originAirport.City, originAirport.Iata),
                    (destinationAirport.City, destinationAirport.Iata));

            } 
            catch (FlightNotFoundException)
            {
                throw new FlightNotFoundException();
            } 
            catch (Exception)
            {
                throw new ArgumentException();
            }
        }
    }
}