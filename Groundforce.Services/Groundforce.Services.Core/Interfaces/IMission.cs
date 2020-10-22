using Groundforce.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Groundforce.Services.Core.Interfaces
{
    public interface IMission
    {
        public int TotalMissionAssigned { get; set; }

        public Task<IEnumerable<AssignedAddresses>> FetchAllOngoingTask(string userId, int page, int page_size);

        public  Task CountRecordsMissionAsync(string userId);

        public  Task<IEnumerable<AssignedAddresses>> PaginatedOngoingTask(string userId, int page, int per_size);
    }
}
