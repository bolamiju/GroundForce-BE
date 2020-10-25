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

        public async Task<bool> IdIsExist(string Id)
        {
            return await _ctx.Request.AnyAsync(x => x.RequestId == Id);
        }
    }
}