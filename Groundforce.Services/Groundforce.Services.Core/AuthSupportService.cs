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

        public AuthSupportService(UserManager<ApplicationUser> userManager, IAgentRepository agentRepository)
        {
            _userManager = userManager;
            _agentRepository = agentRepository;
        }

        // create IdentityUser
        public async Task<IdentityResult> CreateUser(UserToRegisterDTO model)
        {
            // construct the user object
            string defaultPix = "~/images/avarta.jpg";
            var userModel = new ApplicationUser
            {
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Gender = model.Gender,
                PhoneNumber = model.PhoneNumber,
                DOB = model.DOB,
                AvatarUrl = defaultPix
            };

            // create user
            var result = await _userManager.CreateAsync(userModel, model.Password);

            if (result.Succeeded)
            {
                foreach (var role in model.Roles)
                {
                    await _userManager.AddToRoleAsync(userModel, role);
                }

            }

            return result;
        }

        // create Field Agent
        public async Task<bool> CreateFieldAgent(UserToRegisterDTO model, string userId)
        {
            // construct the object
            var userModel = new FieldAgent
            {
                ApplicationUserId = userId,
                ZipCode = model.ZipCode,
                LGA = model.LGA,
                State = model.State,
                Longitude = model.Longitude,
                Latitude = model.Latitude
            };

            // create user
            var result = await _agentRepository.AddAgent(userModel);

            return result;
        }

        public async Task<bool> IsAccountVerified(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
           
            return false;
        }


    }
}
