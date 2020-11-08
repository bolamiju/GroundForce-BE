using Groundforce.Services.Models;
using Groundforce.Services.DTOs;
using System;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Groundforce.Services.Data.Services;

namespace Groundforce.Services.Core
{
    public class AuthSupportService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAgentRepository _agentRepository;
        private readonly IBankRepository _bankRepository;

        public AuthSupportService(UserManager<ApplicationUser> userManager, IAgentRepository agentRepository, IBankRepository bankRepository)
        {
            _userManager = userManager;
            _agentRepository = agentRepository;
            _bankRepository = bankRepository;
        }

        // create IdentityUser
        public async Task<IdentityResult> CreateAppUser(UserWithoutDetailsDTO model, string role)
        {
            string defaultPix = "~/images/avarta.jpg";
            var user = new ApplicationUser
            {
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                DOB = model.DOB,
                LGA = model.LGA,
                PhoneNumber = model.PhoneNumber,
                PlaceOfBirth = model.PlaceOfBirth,
                State = model.State,
                CreatedAt = DateTime.Now,
                Gender = model.Gender,
                HomeAddress = model.HomeAddress,
                AvatarUrl = defaultPix,
                Active = true
            };

            var result =  await _userManager.CreateAsync(user);

            if(result.Succeeded)
                await _userManager.AddToRoleAsync(user, role);

            return result;
        }

        
        // create FieldAgent
        public async Task<bool> CreateFieldAgent(string Id, UserToRegisterDTO model)
        {
            string agentId = "";
            FieldAgent result = null;
            do
            {
                agentId = Guid.NewGuid().ToString();
                result = await _agentRepository.GetAgentById(agentId);
            } while (result != null);

            var agent = new FieldAgent
            {
                FieldAgentId = agentId,
                ApplicationUserId = Id,
                Religion = model.Religion,
                AdditionalPhoneNumber = model.AdditionalPhoneNumber
            };
            return await _agentRepository.AddAgent(agent);
        }


        // create Bank details
        public async Task<bool> CreateBankDetails(string Id, UserToRegisterDTO model)
        {
            string bankId = "";
            BankAccount result = null;
            do
            {
                bankId = Guid.NewGuid().ToString();
                result = await _bankRepository.GetBankDetailsById(bankId);
            } while (result != null);

            var bank = new BankAccount
            {
                AccountId = bankId,
                FieldAgentId = Id,
                AccountName = model.BankName,
                AccountNumber = model.AccountNumber
            };
            return await _bankRepository.AddBankDetail(bank);
        }


        public async Task<ApplicationUser> verifyUser(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null) 
                throw new Exception($"User with id: {Id} was not found");

            if (!user.Active)
                throw new Exception("User's account is not acctive");

            return user;
        }
    }
}
