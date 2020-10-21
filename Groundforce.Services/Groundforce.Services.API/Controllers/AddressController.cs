using System;
using System.Threading.Tasks;
using Groundforce.Services.Data;
using Groundforce.Services.Data.Services;
using Groundforce.Services.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Groundforce.Services.API.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize(Roles = "Admin")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly ILogger<AddressController> _logger;
        private readonly IAddressRepo _addressRepo;
        private readonly AppDbContext _ctx;

        public AddressController(ILogger<AddressController> logger, IAddressRepo addressRepo, AppDbContext ctx)
        {
            _logger = logger;
            _addressRepo = addressRepo;
            _ctx = ctx;
        }

        // PUT api/v1/<AddressController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMission(int id, [FromBody] UpdateAddressDTO model)
        {
            // Check if the id in the request is less than zero
            if (id < 1) return BadRequest("Invalid Credentials");

            try
            {
                /*
                 * Fetch the address model using the address id
                 * If the returned response is null, return a NotFound response
                 */
                var address = await _ctx.Addresses.FirstOrDefaultAsync(x => x.AddressId == id);
                if (address == null) return NotFound("Address does not exist");

                // Objects to be updated using this route
                address.AddressName = model.AddressName;
                address.UpdatedAt = DateTime.Now;

                /*
                 * Services that updates the provided address input
                 * If the response is a true value, return an Ok response
                 * else return a BadRequest response
                 */
                var updatedAddress = await _addressRepo.UpdateAddress(address);
                if (updatedAddress == true) return Ok();

                return BadRequest("Internal Server Error");
            }
            catch (Exception e)
            {
                // Error to be returned, if an error is thrown from this operation
                _logger.LogError(e.Message);
                return BadRequest("Invalid Credentials");
            }
        }
    }
}

