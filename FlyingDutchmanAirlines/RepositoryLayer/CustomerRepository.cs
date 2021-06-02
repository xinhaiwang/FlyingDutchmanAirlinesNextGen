using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FlyingDutchmanAirlines.RepositoryLayer {
    public class CustomerRepository {
        private readonly FlyingDutchmanAirlinesContext _context;

        public CustomerRepository(FlyingDutchmanAirlinesContext context) {
            _context = context;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public CustomerRepository()
        {
            if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
            {
                throw new Exception("This constructor should only be used for testing");
            }
        }

        public async Task<bool> CreateCustomer(string name) {
            if (IsInvalidCustomerName(name)) {
                return false;
            }

            // Perhaps we cannot connect to the database anymore?
            try {
                Customer newCustomer = new Customer(name);
                // using (FlyingDutchmanAirlinesContext context = new FlyingDutchmanAirlinesContext()) {
                using (_context) {
                    _context.Customers.Add(newCustomer);
                    // The context.SaveChangesAsync call is awaited, blocking the current thread
                    // until the changes have been saved.
                    await _context.SaveChangesAsync();
                }
            } catch {
                return false;
            }
            
            // The return of type bool is automatically converted to a type of Task<bool>.
            return true;
        }

        private bool IsInvalidCustomerName(string name) {
            char[] forbiddenCharacters = {'!', '@', '#', '$', '%', '&', '*'};
            return string.IsNullOrEmpty(name) || name.Any(x => forbiddenCharacters.Contains(x));
        }

        public virtual async Task<Customer> GetCustomerByName(string name) {
            if (IsInvalidCustomerName(name)) {
                throw new CustomerNotFoundException();
            }

            return await _context.Customers.FirstOrDefaultAsync(c => c.Name == name)
                   ?? throw new CustomerNotFoundException();
        }
    }
}