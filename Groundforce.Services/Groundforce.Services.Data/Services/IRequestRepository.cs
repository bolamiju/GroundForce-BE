using Groundforce.Services.Models;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public interface IRequestRepository
    {
        Task<Request> GetRequestById(string Id);
        Task<Request> GetRequestByPhone(string Number);
        Task<bool> UpdateRequest(Request model);
        Task<bool> DeleteRequestByPhone(string number);
    }
}