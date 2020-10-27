using Groundforce.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public interface IMissionRepository
    {
        Task<Mission> AddMission(Mission model);
        Task<Mission> UpdateMission(Mission model);

    }
}
