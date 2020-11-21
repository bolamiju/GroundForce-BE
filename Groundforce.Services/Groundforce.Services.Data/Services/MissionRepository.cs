using Groundforce.Services.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Groundforce.Services.Data.Services
{
    public class MissionRepository : IMissionRepository
    {
        private AppDbContext _ctx;
        public int TotalCount { get; set; }


        public MissionRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }


        // add new record to all mission related tables
        public async Task<bool> Add<T>(T model) where T : class
        {
            _ctx.Add(model);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<bool> Update<T>(T model) where T : class
        {
            _ctx.Update(model);
            return await _ctx.SaveChangesAsync() > 0;
        }

        // delete record from any mission related table
        public async Task<bool> Delete<T>(T model) where T : class
        {
            _ctx.Remove(model);
            return await _ctx.SaveChangesAsync() > 0;
        }

        // update assigned mission status to accepted/declined
        public async Task<bool> ChangeMissionStatus(string status, string missionId)
        {
            var mission =  _ctx.Missions.FirstOrDefault(x => x.MissionId == missionId);
            if (mission == null)
                throw new Exception($"Mission with {missionId} not found");

            mission.VerificationStatus = status;
            _ctx.Update(mission);
            return await _ctx.SaveChangesAsync() > 0;
        }


        // get single record of verification item
        public async Task<VerificationItem> GetVerificationItemById(string id)
        {
            return await _ctx.VerificationItems.FirstOrDefaultAsync(x => x.ItemId == id);
        }

        // get all records of verification items
        public async Task<IEnumerable<VerificationItem>> GetAllVerificationItems()
        {
            var items = await _ctx.VerificationItems.ToListAsync();
            TotalCount = items.Count;
            return items;
        }

        // get paginated records of verification items
        public async Task<IEnumerable<VerificationItem>> GetVerificationItemsPaginated(int page, int per_page)
        {
            var items = await GetAllVerificationItems();
            var pagedItems = items.Skip((page - 1) * per_page).Take(per_page).ToList();
            return pagedItems;
        }


        // get all missions for agent
        public async Task<IEnumerable<Mission>> GetMissionsForAgent(string agentId, string status)
        {
            var result = await _ctx.Missions.Where(x => x.FieldAgentId == agentId && x.VerificationStatus == status)
                                            .Include(x => x.VerificationItem).ToListAsync();
            TotalCount = result.Count;
            return result;
        }

        // missions for agents
        public async Task<IEnumerable<Mission>> GetMissionsForAgentPaginated(int page, int per_page, string agentId, string status)
        {
            var missions = await GetMissionsForAgent(agentId, status);
            var pagedItems = missions.Skip((page - 1) * per_page).Take(per_page).ToList();
            return pagedItems;
        }

        // get mission for agent by id
        public async Task<Mission> GetMissionByIdForAgent(string agentId, string missionId)
        {
            return await _ctx.Missions.FirstOrDefaultAsync(x => x.MissionId == missionId && x.FieldAgentId == agentId);
        }

        public async Task<MissionVerified> GetMissionVeriedById(string missionVerifiedId)
        {
            return await _ctx.MissionsVerified.FirstOrDefaultAsync(x => x.Id == missionVerifiedId);
        }

        public async Task<MissionVerified> GetMissionsVeriedByMissionId(string missionId)
        {
            return await _ctx.MissionsVerified.FirstOrDefaultAsync(x => x.MissionId == missionId);
        }

        public async Task<IEnumerable<MissionVerified>> GetMissionsVeried()
        {
            var result = await _ctx.MissionsVerified.ToListAsync();
            TotalCount = result.Count;
            return result;
        }

        public async Task<IEnumerable<MissionVerified>> GetMissionsVeriedPaginated(int page, int per_page)
        {
            var result = await GetMissionsVeried();
            var pagedResult = result.Skip(page - 1).Take(per_page);
            return pagedResult;
        }
    }
}
