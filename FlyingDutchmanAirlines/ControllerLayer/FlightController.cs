using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.ServiceLayer;
using FlyingDutchmanAirlines.Views;
using Microsoft.AspNetCore.Mvc;

namespace FlyingDutchmanAirlines.ControllerLayer
{
    [Route("{controller}")]
    public class FlightController : Controller
    {
        private readonly FlightService _service;

        public FlightController(FlightService service)
        {
            _service = service;
        }

        [HttpGet]
        // The IActionResult interface is implemented by classes such as ActionResult and ContentResult,
        // but in practice, we can leave the determination of which specific class to use to ASP.NET.
        // This is another example of polymorphism, and something we call "Coding to interface".
        
        
        // Implementing logic to change the state of an object inside the actual object is an important
        // tenet of object-oriented design and makes code more readable and maintainable.
        public async Task<IActionResult> GetFlights()
        {
            try
            {
                Queue<FlightView> flights = new Queue<FlightView>();
                
                // Because the FlightService.GetFlights method returns an IAsyncEnumerable<FlightView>
                // and uses the yield return keywords, we don't have to wait for all the processing to be done
                // before being able to see the fruits of our labor.
                
                // As the database returns flights and the service layer populates FlightView,
                // the controller layer receives the instances and adds them to a Queue data
                
                await foreach (FlightView flight in _service.GetFlights())
                {
                    flights.Enqueue(flight);
                }

                return StatusCode((int) HttpStatusCode.OK, flights);
            } 
            catch (FlightNotFoundException)
            {
                return StatusCode((int) HttpStatusCode.NotFound, "No flights were found in the database");
            } 
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError, "An error occurred");
            }
        }

        // Since ASP.NET uses reflection to use the method, the access modifier should be public.
        
        // Routing magic adds the FlightNumber path parameter as a method parameter.
        [HttpGet("{flightNumber}")]
        public async Task<IActionResult> GetFlightByFlightNumber(int flightNumber)
        {
            try
            {
                if (!flightNumber.IsPositive())
                {
                    throw new Exception();
                }

                FlightView flight = await _service.GetFlightByFlightNumber(flightNumber);
                // Use the HttpStatusCode enum and cast its value to an integer.
                return StatusCode((int) HttpStatusCode.OK, flight);
            }
            catch (FlightNotFoundException)
            {
                return StatusCode((int) HttpStatusCode.NotFound, "The flight was not found in the database");
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.BadRequest, "Bad request");
            }
        }
    }
    
}