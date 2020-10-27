using Groundforce.Services.DTOs;
using Groundforce.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public interface IVerificationItemRepository
    {
        int TotalNumberOfItems { get; set; }

        Task<bool> AddItem(VerificationItem model);
        Task<bool> UpdateItem(VerificationItem model);
        Task<IEnumerable<VerificationItem>> GetAllItems();
        Task<IEnumerable<VerificationItem>> GetItemsPaginated(int page, int per_page);
        Task<VerificationItem> GetItemById(string Id);
    }
}
