using Groundforce.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public interface IMissionRepository
    {
        int TotalCount { get; set; }

        Task<bool> Add<T>(T model) where T : class;
        Task<bool> Update<T>(T model) where T : class;
        Task<bool> Delete<T>(T model) where T : class;

        Task<VerificationItem> GetVerificationItemById(string id);
        Task<IEnumerable<VerificationItem>> GetAllVerificationItems();
        Task<IEnumerable<VerificationItem>> GetVerificationItemsPaginated(int page, int per_page);


        Task<Mission> GetMissionByIdForAgent(string agentId, string missionId);
        Task<bool> ChangeMissionStatus(string status, string missionId);
        Task<IEnumerable<Mission>> GetMissionsForAgent(string agentId, string status);
        Task<IEnumerable<Mission>> GetMissionsForAgentPaginated(int page, int per_page, string agentId, string status);

        Task<MissionVerified> GetMissionVeriedById(string missionVerifiedId);
        Task<MissionVerified> GetMissionsVeriedByMissionId(string missionId);
        Task<MissionVerified> GetMissionsVeried();
        Task<MissionVerified> GetMissionsVeriedPaginated(int page, int per_page);



    }
}
