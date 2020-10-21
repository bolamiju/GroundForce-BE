using Groundforce.Services.Core.Interfaces;
using Groundforce.Services.Data;
using Groundforce.Services.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Groundforce.Services.Core.Repositories
{
   public  class MissionRepository : IMission 
    {
        private AppDbContext _ctx;

        public MissionRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }



        public async Task<IEnumerable<AssignedAddresses>> FetchAllOngoingTask(string userId)
        {


            var fieldAgent = await _ctx.FieldAgents.Where(x => x.ApplicationUserId == userId).FirstOrDefaultAsync();
            var allAddress = _ctx.AssignedAddresses.Where(c => c.FieldAgentId == fieldAgent.FieldAgentId && c.IsAccepted && !c.IsVerified).Include(a => a.Address).ToList();


            return allAddress;


        }

    }
}
