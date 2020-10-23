using Groundforce.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public interface IMission
    {
        public int NumberOfOngoingTask { get; set; }
        public int NumberOfOngoingTaskByAgent { get; set; }

        public Task<IEnumerable<AssignedAddresses>> FetchAllOngoingTask(string userId);

        public  Task<IEnumerable<AssignedAddresses>> AllOngoingTaskPaginated(string userId, int page, int per_size);

        List<AssignedAddresses> GetAllMissionsByAgent(int userId);
        List<AssignedAddresses> GetAllMissionsByAgentPaginated(int userId, int page, int per_page);
    }
}
