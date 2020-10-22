using System;
using System.Threading.Tasks;
using Groundforce.Services.Data;
using Groundforce.Services.Data.Services;
using Groundforce.Services.DTOs;
using Groundforce.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Groundforce.Services.API.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly ILogger<AddressController> _logger;
        private readonly IAddressRepo _addressRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public AddressController(ILogger<AddressController> logger, IAddressRepo addressRepo, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _addressRepo = addressRepo;
            _userManager = userManager;
        }

        // PUT api/v1/<AddressController>/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMission(int id, [FromBody] UpdateAddressDTO model)
        {
            // Check if the id in the request is less than zero
            if (id < 1) return BadRequest("Invalid credential");
            
            Address address = null;
            try
            {
                address = await _addressRepo.GetAddressById(id);
                if(address != null)
                {
                     // Objects to be updated using this route
                    address.AddressName = model.AddressName;
                    address.UpdatedAt = DateTime.Now;
                }
            }
            catch (Exception e)
            {
                // Error to be returned, if an error is thrown from this operation
                _logger.LogError(e.Message);
                return BadRequest("Failed to access mission");
            }
          
            //If the returned response is null, return a NotFound response
            if (address == null) return NotFound("Mission does not exist");
           
            bool updatedAddress = false;
            try
            {
                /*
                    * Services that updates the provided address input
                    * If the response is a true value, return an Ok response
                    * else return a BadRequest response
                */
                updatedAddress = await _addressRepo.UpdateAddress(address);
            }
            catch (Exception e)
            {
                // Error to be returned, if an error is thrown from this operation
                _logger.LogError(e.Message);            
                return BadRequest("Failed to update mission");
            }
            
            return Ok("Mission updated");
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddAddress([FromBody] NewAddressDTO addressToAdd)
        {
            if (ModelState.IsValid)
            {
                var agent = await _userManager.GetUserAsync(User);

                if (agent == null) return NotFound("User not found");

                Address updateAddress;
                var newAddress = new Address
                {
                    ApplicationUserId = agent.Id,
                    AddressName = addressToAdd.AddressName
                };

                try
                {
                    updateAddress = await _addressRepo.AddAddress(newAddress);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return BadRequest("Failed to add the address. Try again");
                }

                var fetchAddressId = await _addressRepo.GetAddressById(updateAddress.AddressId);

                if (fetchAddressId != null)
                {
                    var addressToReturn = new ReturnedAddressDTO
                    {
                        AddressId = fetchAddressId.AddressId,
                        AddressName = fetchAddressId.AddressName,
                        ApplicationUserId = fetchAddressId.ApplicationUserId,
                        CreatedAt = fetchAddressId.CreatedAt
                    };

                    return Ok(addressToReturn);
                }
            }
            return BadRequest(ModelState);
        }
    }
}