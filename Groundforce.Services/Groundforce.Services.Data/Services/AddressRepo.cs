using Groundforce.Services.DTOs;
using Groundforce.Services.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
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
    }
}