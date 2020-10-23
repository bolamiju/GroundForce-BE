using Groundforce.Services.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{


    public class AgentRepository : IAgentRepository
    {
        private readonly AppDbContext _ctx;

        public AgentRepository(AppDbContext context)
        {
            _ctx = context;
        }
        public async Task<FieldAgent> GetAgentById(int id)
        {
            return await _ctx.FieldAgents.FirstOrDefaultAsync(agent => agent.FieldAgentId == id);
        }
    }
}
