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
        private readonly AppDbContext _ctx;
        public AddressRepo(AppDbContext context)
        {
            _ctx = context;
        }
        public async Task<Address> UpdateAddress(int id, UpdateAddressDTO model)
        {
            var address = await _ctx.Addresses.FirstOrDefaultAsync(x => x.AddressId == id);
            if (address == null) return null;

            address.AddressName = model.AddressName;
            address.UpdatedAt = DateTime.Now;

            _ctx.Addresses.Update(address);
            await _ctx.SaveChangesAsync();
            return address;
        }
    }
}