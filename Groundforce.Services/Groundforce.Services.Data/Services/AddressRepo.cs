using Groundforce.Services.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public class AddressRepo : IAddressRepo
    {
        // Injection of the DbContext in the Repositiory class
        private readonly AppDbContext _ctx;
        public AddressRepo(AppDbContext context)
        {
            _ctx = context;
        }

        public async Task<Address> AddAddress(Address newAddress)
        {
            await _ctx.Addresses.AddAsync(newAddress);
            await _ctx.SaveChangesAsync();

            return newAddress;
        }

        public async Task<Address> GetAddressById(int Id)
        {
            // return a single address by Id
            return await _ctx.Addresses.FirstOrDefaultAsync(x => x.AddressId == Id);
        }

        // Method to update the address of a mission
        public async Task<bool> UpdateAddress(Address model)
        {
            /*
             * Update the address table with the model provided from the controller
             * Save changes to the database and return a bool value if it was successful or not
            */
            _ctx.Addresses.Update(model);
            var result = await _ctx.SaveChangesAsync();
            if (result > 0) return true;
            return false;
        }

        public async Task<bool> UpdateAcceptedStatus(int id, bool accept)
        {
            var assignedAddress = _ctx.AssignedAddresses.SingleOrDefault(add => add.Id == id);
            assignedAddress.Accepted = accept;
            assignedAddress.UpdatedAt = DateTime.Now;

            //patch the 'Accepted' column of the assignedAddress table
            _ctx.AssignedAddresses.Update(assignedAddress);

            var result = await _ctx.SaveChangesAsync();

            // if the number of row affected is greater or equal to 1, that record was successfully patched. Else, there was an error
            if (result >= 1) return true;

            return false;
        }

        public async Task<Address> GetAddress(int id)
        {
            var address = await _ctx.Addresses.FirstOrDefaultAsync(address => address.AddressId == id);
            return address;
        }
    }
}