using System;
using System.Threading.Tasks;
using Groundforce.Services.Data.Services;
using Groundforce.Services.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Groundforce.Services.API.Controllers
{
    //[Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly ILogger<AddressController> _logger;
        private readonly IAddressRepo _addressRepo;

        public AddressController(ILogger<AddressController> logger, IAddressRepo addressRepo)
        {
            _logger = logger;
            _addressRepo = addressRepo;
        }

        // PUT api/v1/<AddressController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMission(int id, [FromBody] UpdateAddressDTO model)
        {
            if (id < 0) return BadRequest();

            try
            {
                var updatedAddress = await _addressRepo.UpdateAddress(id, model);
                if (updatedAddress == null) return NotFound();

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest();
            }
        }
    }
}

