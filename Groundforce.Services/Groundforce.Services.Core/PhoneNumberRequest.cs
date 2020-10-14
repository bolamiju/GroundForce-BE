using Groundforce.Common.Utilities;
using Groundforce.Services.Data;
using Groundforce.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Groundforce.Services.Core
{
    public class PhoneNumberRequest
    {
        private static AppDbContext _ctx;
        //public PhoneNumberRequest(AppDbContext ctx)
        //{
        //    _ctx = ctx;
        //}

        public static async Task<PhoneNumberStatus> CheckPhoneNumber(string phoneNumber, AppDbContext _ctx)
        {
            var model = _ctx.Request.Where(x => x.PhoneNumber == phoneNumber).FirstOrDefault();

            // Check status of phone number
            if (model != null)
            {
                if (model.IsVerified)
                {
                    return PhoneNumberStatus.Verified;
                }
                else
                {
                    if (model.RequestAttempt > 4)
                    {
                        model.IsBlocked = true;
                        model.UpdatedAt = DateTime.Now;
                        _ctx.Request.Update(model);
                        await _ctx.SaveChangesAsync();
                        return PhoneNumberStatus.Blocked;
                    }
                    else
                    {
                        model.RequestAttempt++;
                        model.UpdatedAt = DateTime.Now;
                        _ctx.Request.Update(model);
                        await _ctx.SaveChangesAsync();
                        return PhoneNumberStatus.ProcessRequest;
                    }
                }
            }
            else
            {
                await _ctx.Request.AddAsync(new Models.Request
                {
                    PhoneNumber = phoneNumber
                });
                await _ctx.SaveChangesAsync();
                return PhoneNumberStatus.ProcessRequest;
            }       
        }
    }
}
