using Groundforce.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public interface IPasswordVerificationRepository
    {
        Task<bool> AddPasswordVerification(PasswordVerification model);
        Task<PasswordVerification> GetPasswordVerificationByEmail(string email);
        Task<PasswordVerification> GetPasswordVerificationByEmailAndToken(string email, string token);
        Task<bool> UpdatePasswordVerification(PasswordVerification model);
    }
}
