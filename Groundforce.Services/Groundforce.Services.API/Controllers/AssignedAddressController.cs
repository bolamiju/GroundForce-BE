using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Groundforce.Common.Utilities;
using Groundforce.Services.Data;
using Groundforce.Services.Data.Services;
using Groundforce.Services.DTOs;
using Groundforce.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Groundforce.Services.API.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    public class AssignedAddressController : ControllerBase
    {
        private AppDbContext _ctx;
        private readonly IMission _mission;
        private readonly IAddressRepo _addressRepo;
        private readonly ILogger<AssignedAddressController> _logger;
        private readonly IAgentRepository _agentRepo;
        private readonly int perPage = 10;

        public AssignedAddressController(AppDbContext ctx, IMission mission, IAddressRepo addressRepo, ILogger<AssignedAddressController> logger, IAgentRepository agentRepo)
        {
            _ctx = ctx;
            _mission = mission;
            _addressRepo = addressRepo;
            _logger = logger;
            _agentRepo = agentRepo;
        }

        [HttpGet("{userId}/ongoing/{page}")]
        [Authorize(Roles = "Agent, Admin")]
        public async Task<IActionResult> GetAllOngoingMissionPaginated(string userId, int page)
        {
            // Fetch paginated result
            IEnumerable<AssignedAddresses> allAddress;
            try
            {
               allAddress = await _mission.AllOngoingTaskPaginated(userId, page, perPage);
            }
            catch (Exception)
            {
                return BadRequest("Invalid Detail");
            }

            //  Reshape result
            var missions = new List<MissionDTO>();

            foreach (var address in allAddress)
            {
                var Ongoingmission = new MissionDTO
                {
                    Remarks = address.Remarks,
                    LandMark = address.Landmark,
                    BusStop = address.BusStop,
                    AddressName = address.Address.AddressName,
                    BuildColor = address.BuildingColor
                };
                missions.Add(Ongoingmission);
            }

            page = page > _mission.NumberOfOngoingTask ? _mission.NumberOfOngoingTask : page;
            page = page <= 0 ? 1 : page;

            // new dto that contains pagination details 
            var pageMission = new MissionPaginatedDTO();
            pageMission.Pagination = PaginationUtil.Paginate(page, perPage, _mission.NumberOfOngoingTask);
            pageMission.Data = missions;

            return Ok(pageMission);
        }


        [Authorize(Roles = "Agent, Admin")]
        [HttpPatch("{id}/accepted")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] AssignedAddressUpdateDTO columnUpdate)
        {
            if (id < 1) return BadRequest("invalid parameter name");

            if (ModelState.IsValid)
            {
                try
                {
                    var updateAddress = await _addressRepo.UpdateAcceptedStatus(id, columnUpdate.Accepted);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return BadRequest("Could not update the address");
                }

                return Ok("Status Updated");
            }
            return BadRequest(ModelState);
        }

        [HttpGet]
        [Authorize(Roles = "Agent")]
        [Route("{userId:int}/reports/{pageNumber:int}")]
        public async Task<IActionResult> FetchMission(int userId, int pageNumber)
        {
            FetchPaginatedMissionDTO missionsToReturn;
            List<AssignedAddresses> missions;
            try
            {
                var agent = await _agentRepo.GetAgentById(userId);
                if (agent == null)
                {
                    return BadRequest("Agent does not exist");
                }

                missions =  _mission.GetAllMissionsByAgent(userId);

                pageNumber = pageNumber > 0 ? pageNumber : 1;
                var totalMissions = missions.Count;

                var numberOfMissionsToSkip = (pageNumber - 1) * perPage;

                //assigns properties for the return value
                missionsToReturn = new FetchPaginatedMissionDTO
                {
                    PaginationItems = PaginationUtil.Paginate(pageNumber, perPage, totalMissions),
                    Data = _mission.GetAllMissionsByAgentPaginated(userId, pageNumber, perPage)
                };
            }
            catch (Exception)
            {
                return BadRequest("Unable to fetch data");
            }
            return Ok(missionsToReturn);
        }
    }
}