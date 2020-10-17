using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Groundforce.Common.Utilities;
using Groundforce.Services.Data;
using Groundforce.Services.Models;

namespace Groundforce.Services.Core
{
    public class PhoneNumberResource
    {
        //DbContext class
        private readonly AppDbContext _ctx;

        public PhoneNumberResource(AppDbContext ctx)
        {
            _ctx = ctx;
        }
        //method to verify the phone number before sending OTP
        public async Task<PhoneNumberStatus> CheckPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber)) return PhoneNumberStatus.InvalidRequest; 
            //get the number from the database
            var model = _ctx.Request.FirstOrDefault(user => user.PhoneNumber == phoneNumber);

            if (model != null)
            {
                if (model.IsVerified)
                {
                    return PhoneNumberStatus.Verified;
                }
                if (model.RequestAttempt < 4)
                {
                    model.RequestAttempt += 1;
                    model.UpdatedAt = DateTime.Now;
                    await _ctx.SaveChangesAsync();
                    return PhoneNumberStatus.ValidRequest;
                }

                model.IsBlock = true;
                model.UpdatedAt = DateTime.Now;
                await _ctx.SaveChangesAsync();
                return PhoneNumberStatus.Blocked;

            }
            //adds number to the database
            await _ctx.AddAsync(new Request()
            {
                PhoneNumber = phoneNumber,
                RequestAttempt = 1
            });

            await _ctx.SaveChangesAsync();


            return PhoneNumberStatus.ValidRequest;
        }
    }
}
