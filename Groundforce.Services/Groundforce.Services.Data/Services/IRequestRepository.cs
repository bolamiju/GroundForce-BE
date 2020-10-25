using Groundforce.Services.Models;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public interface IRequestRepository
    {
        Task<bool> IdIsExist(string Id);
    }
}