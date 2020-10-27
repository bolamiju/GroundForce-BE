using Groundforce.Services.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public class BankRepository : IBankRepository
    {
        private readonly AppDbContext _ctx;

        public BankRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<bool> AddBankDetail(BankAccount model)
        {
            await _ctx.BankAccounts.AddAsync(model);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<BankAccount> GetBankDetailsById(string Id)
        {
            return await _ctx.BankAccounts.FirstOrDefaultAsync(x => x.AccountId == Id);
        }
    }
}
