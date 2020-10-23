using Groundforce.Services.Data;
using Groundforce.Services.DTOs;
using Groundforce.Services.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
   public  class MissionRepository : IMission 
    {
        private AppDbContext _ctx;
        public int NumberOfOngoingTask  { get; set; }
        public int NumberOfOngoingTaskByAgent  { get; set; }

        public MissionRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IEnumerable<AssignedAddresses>> FetchAllOngoingTask(string userId)
        {
            // get all address 
            var fieldAgent = await _ctx.FieldAgents.Where(x => x.ApplicationUserId == userId).FirstOrDefaultAsync();
            var allAddress = _ctx.AssignedAddresses.Where(c => c.FieldAgentId == fieldAgent.FieldAgentId && c.IsAccepted && !c.IsVerified).Include(a => a.Address).ToList();

            NumberOfOngoingTask = allAddress.Count;
            return allAddress;
        }

        public async Task<IEnumerable<AssignedAddresses>> AllOngoingTaskPaginated(string userId, int page, int per_size)
        {
            var per_page = per_size;

            // get all address 
            var fieldAgent = await _ctx.FieldAgents.Where(x => x.ApplicationUserId == userId).FirstOrDefaultAsync();
            var allAddress = _ctx.AssignedAddresses.Where(c => c.FieldAgentId == fieldAgent.FieldAgentId && c.IsAccepted && !c.IsVerified).Include(a => a.Address).Skip((page - 1) * per_page).Take(per_page).ToList();
            await FetchAllOngoingTask(userId);
            return allAddress;
        }

        public List<AssignedAddresses> GetAllMissionsByAgent(int userId)
        {
            var addressesAssignedToUser = _ctx.AssignedAddresses.Where(agent => agent.FieldAgentId == userId).ToList();
            NumberOfOngoingTaskByAgent = addressesAssignedToUser.Count;
            return addressesAssignedToUser;
        }
        public List<AssignedAddresses> GetAllMissionsByAgentPaginated(int userId, int page, int per_page)
        {
            var addressesAssignedToUser = _ctx.AssignedAddresses.Where(agent => agent.FieldAgentId == userId).Skip((page - 1) * per_page).Take(per_page).ToList();
           GetAllMissionsByAgent(userId);
            return addressesAssignedToUser;
        }

    }
}
