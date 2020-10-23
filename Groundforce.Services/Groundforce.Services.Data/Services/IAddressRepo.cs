using Groundforce.Services.Models;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public interface IAddressRepo
    {
        Task<bool> UpdateAddress(Address model);
        Task<Address> GetAddressById(int Id);
        Task<Address> AddAddress(Address newAddress);
        Task<bool> UpdateAcceptedStatus(int id, bool change);
        Task<Address> GetAddress(int id);
    }
}