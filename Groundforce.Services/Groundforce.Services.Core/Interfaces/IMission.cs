using Groundforce.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Groundforce.Services.Core.Interfaces
{
    public interface IMission
    {


        public Task<IEnumerable<AssignedAddresses>> FetchAllOngoingTask(string userId);
    }
}
