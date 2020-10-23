using Groundforce.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public interface IAgentRepository
    {
        Task<FieldAgent> GetAgentById(int Id);
    }
}
