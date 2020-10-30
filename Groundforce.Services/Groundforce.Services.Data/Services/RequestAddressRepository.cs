using Groundforce.Services.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public class RequestRepository : IRequestRepository
    {
        // Injection of the DbContext in the Repositiory class
        private readonly AppDbContext _ctx;
        public RequestRepository(AppDbContext context)
        {
            _ctx = context;
        }

        public async Task<bool> DeleteRequestByPhone(string number)
        {
            var request = await _ctx.Request.FirstOrDefaultAsync(x => x.PhoneNumber == number);
            _ctx.Request.Remove(request);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<Request> GetRequestById(string Id)
        {
            return await _ctx.Request.FirstOrDefaultAsync(x => x.RequestId == Id);
        }

        public async Task<Request> GetRequestByPhone(string Number)
        {
            return await _ctx.Request.FirstOrDefaultAsync(item => item.PhoneNumber == Number); 
        }

        public async Task<bool> UpdateRequest(Request model)
        {
            _ctx.Request.Update(model);
            return await _ctx.SaveChangesAsync() > 0;
        }
    }
}