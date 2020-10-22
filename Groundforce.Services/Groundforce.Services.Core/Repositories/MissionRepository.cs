using Groundforce.Services.Core.Interfaces;
using Groundforce.Services.Data;
using Groundforce.Services.DTOs;
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
        public int TotalMissionAssigned  { get; set; }

        public MissionRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }



        public async Task<IEnumerable<AssignedAddresses>> FetchAllOngoingTask(string userId, int page, int per_size)
        {
            var per_page = per_size;

            // get all address 
            var fieldAgent = await _ctx.FieldAgents.Where(x => x.ApplicationUserId == userId).FirstOrDefaultAsync();
            var allAddress = _ctx.AssignedAddresses.Where(c => c.FieldAgentId == fieldAgent.FieldAgentId && c.IsAccepted && !c.IsVerified).Include(a => a.Address).ToList();

            await CountRecordsMissionAsync(userId);

            return allAddress;


        }

        public async Task<IEnumerable<AssignedAddresses>> PaginatedOngoingTask(string userId, int page, int per_size)
        {
            var per_page = per_size;

            // get all address 
            var fieldAgent = await _ctx.FieldAgents.Where(x => x.ApplicationUserId == userId).FirstOrDefaultAsync();
            var allAddress = _ctx.AssignedAddresses.Where(c => c.FieldAgentId == fieldAgent.FieldAgentId && c.IsAccepted && !c.IsVerified).Include(a => a.Address).ToList();

            await CountRecordsMissionAsync(userId);
            page = page > TotalMissionAssigned ? TotalMissionAssigned : page;
            page = page < 0 ? 1 : page;
            var pageData = allAddress.Skip((page - 1) * per_page).Take(per_page);

            return pageData;
        }


        public async Task CountRecordsMissionAsync(string userId)
        {
            var fieldAgent = await _ctx.FieldAgents.Where(x => x.ApplicationUserId == userId).FirstOrDefaultAsync();
            var allAddress = _ctx.AssignedAddresses.Where(c => c.FieldAgentId == fieldAgent.FieldAgentId && c.IsAccepted && !c.IsVerified).ToList();

          TotalMissionAssigned =  allAddress.Count();
        }

    }
}
