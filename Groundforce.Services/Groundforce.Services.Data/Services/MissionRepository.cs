using Groundforce.Services.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public class MissionRepository : IMissionRepository
    {
        private AppDbContext _ctx;

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

        // update assigned mission status to accepted/declined
        public async Task<bool> ChangeAssignedMissionStatus(string status, string missionId)
        {
            var mission =  _ctx.Missions.FirstOrDefault(x => x.MissionId == missionId);
            if (mission == null)
                throw new Exception($"Mission with {missionId} not found");

            mission.VerificationStatus = status;
            _ctx.Update(mission);
            return await _ctx.SaveChangesAsync() > 0;
        }

        // delete record from any mission related tables
        public async Task<bool> Delete<T>(T model) where T : class
        {
            _ctx.Remove(model);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public Task<IEnumerable<VerificationItem>> GetAllMissionsForAgent()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VerificationItem>> GetAllVerificationItems()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VerificationItem>> GetAllVerifiedMissions()
        {
            throw new NotImplementedException();
        }

        public Task<VerificationItem> GetMissionForAgentById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VerificationItem>> GetMissionsForAgentPaginated()
        {
            throw new NotImplementedException();
        }

        public Task<VerificationItem> GetVerificationItemById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VerificationItem>> GetVerificationItemsPaginated()
        {
            throw new NotImplementedException();
        }

        public Task<VerificationItem> GetVerifiedMissionById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VerificationItem>> GetVerifiedMissionsPaginated()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update<T>(T model) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
