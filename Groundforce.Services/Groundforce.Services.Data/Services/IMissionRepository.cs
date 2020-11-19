using Groundforce.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public interface IMissionRepository
    {
        Task<bool> Add<T>(T model) where T : class;
        Task<bool> Update<T>(T model) where T : class;
        Task<bool> Delete<T>(T model) where T : class;

        Task<VerificationItem> GetVerificationItemById(string id);
        Task<IEnumerable<VerificationItem>> GetAllVerificationItems();
        Task<IEnumerable<VerificationItem>> GetVerificationItemsPaginated();


        Task<VerificationItem> GetMissionForAgentById(string id);
        Task<bool> ChangeAssignedMissionStatus(string status, string missionId);
        Task<IEnumerable<VerificationItem>> GetAllMissionsForAgent();
        Task<IEnumerable<VerificationItem>> GetMissionsForAgentPaginated();


        Task<VerificationItem> GetVerifiedMissionById(string id);
        Task<IEnumerable<VerificationItem>> GetAllVerifiedMissions();
        Task<IEnumerable<VerificationItem>> GetVerifiedMissionsPaginated();

    }
}
