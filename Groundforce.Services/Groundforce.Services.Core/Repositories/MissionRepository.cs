using Groundforce.Services.Core.Interfaces;
using Groundforce.Services.Data;
using Groundforce.Services.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Groundforce.Services.Core.Repositories
{
   public  class MissionRepository : IMission 
    {
        private AppDbContext _ctx;

        public MissionRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }



        public async Task<IEnumerable<AssignedAddresses>> FetchAllOngoingTask(string userId, int page, int per_size)
        {
            var per_page = per_size;

            // get all address 
            var fieldAgent = await _ctx.FieldAgents.Where(x => x.ApplicationUserId == userId).FirstOrDefaultAsync();
            var allAddress = _ctx.AssignedAddresses.Where(c => c.FieldAgentId == fieldAgent.FieldAgentId && c.IsAccepted && !c.IsVerified).Include(a => a.Address).ToList();


            // total pages 
            var totalPages = (int)Math.Ceiling(decimal.Divide(allAddress.Count(), per_page));

            // check if greater than totalpages 
            page = page > totalPages ? totalPages : page;
            //check if less than  0
            page = page < 0 ? 1 : page;
            //  paginated datas
            var pageData = allAddress.Skip((page - 1) * per_page).Take(per_page);


            return allAddress;


        }

    }
}
