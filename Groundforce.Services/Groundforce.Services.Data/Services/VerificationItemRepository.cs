using Groundforce.Services.DTOs;
using Groundforce.Services.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public class VerificationItemRepository : IVerificationItemRepository
    {
        private readonly AppDbContext _ctx;
        public int TotalNumberOfItems { get; set; }

        public VerificationItemRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<bool> AddItem(VerificationItem model)
        {
            await _ctx.VerificationItems.AddAsync(model);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<VerificationItem>> GetAllItems()
        {
            var items = await _ctx.VerificationItems.ToListAsync();
            TotalNumberOfItems = items.Count;
            return items;
        }

        public async Task<VerificationItem> GetItemById(string Id)
        {
            return await _ctx.VerificationItems.FirstOrDefaultAsync(x => x.ItemId == Id);
        }

        public async Task<IEnumerable<VerificationItem>> GetItemsPaginated(int page, int per_page)
        {
            var items = await GetAllItems();
            var pagedItems = items.Skip((page - 1) * per_page).Take(per_page).ToList();
            return pagedItems;
        }

        public async Task<bool> UpdateItem(VerificationItem model)
        {
            _ctx.VerificationItems.Update(model);
            return await _ctx.SaveChangesAsync() > 0;
        }
    }
}
