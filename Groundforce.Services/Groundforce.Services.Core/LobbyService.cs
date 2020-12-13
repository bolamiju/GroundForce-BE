using System;
using System.Linq;
using System.Threading.Tasks;
using Groundforce.Common.Utilities;
using Groundforce.Services.Data;
using Groundforce.Services.Models;

namespace Groundforce.Services.Core
{
    public class LobbyService
    {
        //DbContext class
        private readonly AppDbContext _ctx;

        public LobbyService(AppDbContext ctx)
        {
            _ctx = ctx;
        }
        //method to verify the phone number before sending OTP
        public async Task CheckPhoneNumber(Request model)
        {
            if (model.Status == "confirmed")
            {
                throw new Exception("Number is already confirmed");
            }
            
            if (model.Status == "blocked")
            {
                throw new Exception("Number is blocked due to attempts out of limit, please contact admin");
            }

            if (model.RequestAttempt >= 4)
            {
                model.Status = "blocked";
                model.UpdatedAt = DateTime.Now;
                await _ctx.SaveChangesAsync();
                throw new Exception("Number is blocked due to attempts out of limit, please contact admin"); ;
            }
            
        }
    }
}
