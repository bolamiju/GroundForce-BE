using Groundforce.Services.Models;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public interface IAgentRepository
    {
        Task<bool> AddAgent(FieldAgent model);
        Task<FieldAgent> GetAgentById(string Id);
        Task<bool> DeleteAgent(FieldAgent model);
    }
}
