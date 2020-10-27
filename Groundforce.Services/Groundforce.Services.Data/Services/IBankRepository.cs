using Groundforce.Services.Models;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public interface IBankRepository
    {
        Task<bool> AddBankDetail(BankAccount model);
        Task<BankAccount> GetBankDetailsById(string Id);
    }
}
