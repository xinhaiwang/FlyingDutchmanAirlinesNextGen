using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FlyingDutchmanAirlines_Tests.Stubs {
    class FlyingDutchmanAirlinesContext_Stub : FlyingDutchmanAirlinesContext {
        public FlyingDutchmanAirlinesContext_Stub(DbContextOptions<FlyingDutchmanAirlinesContext> options) :
            base(options) {
            base.Database.EnsureCreated();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
            IEnumerable<EntityEntry> pendingChanges = ChangeTracker.Entries().Where(e => e.State == EntityState.Added);
            IEnumerable<Booking> bookings = pendingChanges.Select(e => e.Entity).OfType<Booking>();
            if (bookings.Any(b => b.CustomerId != 1)) {
                throw new Exception("Database Error!");
            }

            IEnumerable<Airport> airports = pendingChanges.Select(e => e.Entity).OfType<Airport>();
            if (airports.Any(a => a.AirportId == 10)) {
                throw new Exception("Database Error!");
            }
            
            await base.SaveChangesAsync(cancellationToken);
            return 1;
        }
    }
}