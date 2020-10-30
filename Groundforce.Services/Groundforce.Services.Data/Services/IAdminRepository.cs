using System.Threading.Tasks;
using Groundforce.Services.Models;

namespace Groundforce.Services.Data.Services
{
    public interface IAdminRepository
    {
        Task<bool> AddAdmin(Admin model);
        Task<Admin> GetAdminById(string Id);
        Task<bool> DeleteAdmin(Admin model);
    }
}
