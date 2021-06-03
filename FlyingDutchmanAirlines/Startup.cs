using System.Runtime.InteropServices.ComTypes;
using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.ServiceLayer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace FlyingDutchmanAirlines
{
    class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Transient: A new instance every time a dependency is used.
            // Scoped: one instance across the lifetime of a request.
            // Singleton: one instance across the entire lifetime of the service.
            
            // Because transient dependencies are the most common, and easiest to use,
            // types of dependency injection, we shall fall in line.
            services.AddTransient(typeof(FlightService), typeof(FlightService));
            services.AddTransient(typeof(BookingService), typeof(BookingService));
            services.AddTransient(typeof(FlightRepository), typeof(FlightRepository));
            services.AddTransient(typeof(AirportRepository), typeof(AirportRepository));
            services.AddTransient(typeof(BookingRepository), typeof(BookingRepository));
            services.AddTransient(typeof(CustomerRepository), typeof(CustomerRepository));

            services.AddDbContext<FlyingDutchmanAirlinesContext>(ServiceLifetime.Transient);
            services.AddTransient(typeof(FlyingDutchmanAirlinesContext), typeof(FlyingDutchmanAirlinesContext));

            services.AddSwaggerGen();

        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());

            app.UseSwagger();
            app.UseSwaggerUI(swagger =>
                swagger.SwaggerEndpoint("/swagger/v1/swagger.json", "Flying Dutchman Airlines"));
        }
    }
}