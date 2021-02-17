using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public interface IBankDetailsService
    {
        Task<string> GetAccountName(string accountNumber, string bankCode);
    }
}
