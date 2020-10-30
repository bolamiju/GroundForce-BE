using Groundforce.Services.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Groundforce.Services.Data.Services
{
    public interface IBankRepository
    {
        Task<bool> AddBankDetail(BankAccount model);
        Task<BankAccount> GetBankDetailsById(string Id);
        Task<List<BankAccount>> GetBankDetailsByAgent(string agentId);
        Task<bool> DdeleteBankDetail(BankAccount model);

    }
}
