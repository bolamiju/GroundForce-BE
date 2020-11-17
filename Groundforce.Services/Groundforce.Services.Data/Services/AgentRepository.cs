using Groundforce.Services.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public class AgentRepository : IAgentRepository
    {
        private readonly AppDbContext _ctx;

        public AgentRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }
        public async Task<bool> AddAgent(FieldAgent model)
        {
            await _ctx.FieldAgents.AddAsync(model);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAgent(FieldAgent model)
        {
            _ctx.FieldAgents.Remove(model);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<FieldAgent> GetAgentById(string Id)
        {
            return await _ctx.FieldAgents.FirstOrDefaultAsync(x => x.ApplicationUserId == Id);
        }

        public async Task<bool> UpdateAgent(FieldAgent model)
        {
            _ctx.FieldAgents.Update(model);
            return await _ctx.SaveChangesAsync() > 0;
        }
    }
}
